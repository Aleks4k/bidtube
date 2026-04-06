using bidtube.Application.Auctions.Contracts;
using bidtube.Domain.Data;
using SixLabors.ImageSharp;
using bidtube.Domain.Entities;
using bidtube.Application.Auctions.DTO;
using Microsoft.EntityFrameworkCore;
using bidtube.Application.Auctions.Enum;
using SixLabors.ImageSharp.Formats.Jpeg;
using bidtube.Application.Categories.Mappers;
using bidtube.Application.Auctions.Mappers;
using bidtube.Application.Bids.Mappers;
using bidtube.Application.Categories.Contracts;
using bidtube.Application.Notifications.DTO;
using bidtube.Application.Users.Contracts;

namespace bidtube.Infrastructure.Repository
{
    public class AuctionRepository : IAuction
    {
        private readonly AppDbContext _context;
        private readonly ICategory _categoryRepo;
        private readonly IUser _userRepo;
        public AuctionRepository(AppDbContext context, ICategory categoryRepository, IUser userRepository)
        {
            _context = context;
            _categoryRepo = categoryRepository;
            _userRepo = userRepository;
        }
        public async Task AddAuction(Auction auction, CancellationToken cancellationToken)
        {
            await _context.Auctions.AddAsync(auction, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<List<AddNotificationDto>> EndAuctions()
        {
            var auctionsToEnd = await _context.Auctions
                .Include(x => x.Bids)
                .Include(x => x.Images)
                .Where(x => x.date_of_expiration <= DateTime.UtcNow).ToListAsync();
            var auctionsToInsertInHistory = auctionsToEnd.Select(x => x.ToHistoryEntity()).ToList();
            List<AddNotificationDto> response = new List<AddNotificationDto>();
            foreach(var auction in auctionsToInsertInHistory) //Treba još da dodamo pobednika.
            {
                auction.user_bought_id = auction.Bids.OrderByDescending(x => x.amount).Select(x => (int?) x.user_id).FirstOrDefault(); //Konvertujemo int u nullable int jer ako dobijemo 0 posle ne možemo da insertujemo u bazu zbog stranog ključa.
                var highestBid = auction.Bids.OrderByDescending(x => x.amount).Select(x => x.amount).FirstOrDefault();
                var userWon = auction.user_bought_id.HasValue ? await _userRepo.GetUserUsernameByID(auction.user_bought_id.Value) : string.Empty;
                response.Add(new AddNotificationDto
                {
                    user_id = auction.user_id,
                    action = auction.user_bought_id.HasValue ? String.Concat("msg(", auction.user_bought_id.Value, ")") : string.Empty,
                    title = "Auction ended!",
                    description = auction.user_bought_id.HasValue ? $"The auction {auction.title} has ended.\n{userWon} won with the highest bid of €{highestBid}.\nTo contact {userWon}, click button below." : $"The auction {auction.title} has ended, but there were no bids.\nThank you for your interest!"
                });
                if (auction.user_bought_id.HasValue)
                {
                    response.Add(new AddNotificationDto
                    {
                        user_id = auction.user_bought_id.Value,
                        action = String.Concat("msg(", auction.user_id, ")"),
                        title = "Auction won!",
                        description = $"Congratulations! You won the auction {auction.title} with the highest bid of €{highestBid}.\nClick button below to contact the seller and finalize the transaction."
                    });
                }
            }
            _context.Auctions.RemoveRange(auctionsToEnd);
            await _context.Auctions_history.AddRangeAsync(auctionsToInsertInHistory);
            await _context.SaveChangesAsync();
            return response;
        }
        public async Task<AuctionsBidCheckDataDto?> GetAuctionForBidCheckById(int auction_id)
        {
            var response = await _context.Auctions
                .AsNoTracking()
                .Where(x => x.ID == auction_id)
                .Select(auction => new AuctionsBidCheckDataDto
                {
                    ID = auction.ID,
                    user_id = auction.user_id,
                    startPrice = auction.startPrice,
                    date_of_expiration = auction.date_of_expiration,
                })
                .FirstOrDefaultAsync();
            if(response != null) { 
                response.top_bid = await _context.Bids
                    .AsNoTracking()
                    .Where(x => x.auction_id == auction_id)
                    .OrderByDescending(x => x.amount)
                    .Select(x => x.ToDetailsWithUserIDDto())
                    .FirstOrDefaultAsync();
            }
            return response;
        }
        public async Task<AuctionsReturnDto> GetAuctions(GetAuctionsQueryDto auction)
        {
            var response = new AuctionsReturnDto();
            var row_num = 0;
            List<int> categories = new List<int>();
            if(auction.category_id != 0)
            {
                categories = await _categoryRepo.GetSubcategoriesForCategoryId(auction.category_id!.Value);
                row_num = await _context.Auctions.AsNoTracking().Where(x => categories.Contains(x.category_id)).CountAsync();
            } 
            else
            {
                row_num = await _context.Auctions.AsNoTracking().CountAsync();
            }
            response.total_rows = row_num;
            List<Auction> db_resp = new List<Auction>();
            /*
                SortType.TimePosted - Za sada može da radi na ID jer ID u bazi direktno je povezan i sa početkom aukcije u smislu
                da što je veći ID to je aukcija kasnije počela.
                Ako se ikada bude dodavao sistem za odloženo startovanje aukcije u tom momentu ovaj sort će morati da se promeni po datumu početka.
            */
            if (auction.sort_type == SortType.TimePosted)
            {
                //U ovom if bloku menjamo order type zbog filtriranja dok ćemo dole zbog finalnog sortiranja poslednjih 10 stavki da vratimo na prvobitan sort type da bi dobili normalne podatke a ne da nam se i oni orderuju u surotnom redosledu.
                if(auction.pagination_direction_type == PaginationDirectionType.Backward)
                {
                    auction.order_type = auction.order_type == OrderType.Descending ? OrderType.Ascending : OrderType.Descending;
                }
                var query = _context.Auctions.AsNoTracking();
                if(auction.auction_id_filter != 0) { //Ako nije inicijalna pretraga.
                    query = auction.order_type == OrderType.Ascending
                        ? query.Where(x => x.ID > auction.auction_id_filter)
                        : query.Where(x => x.ID < auction.auction_id_filter);
                }
                if (auction.category_id != 0)
                {
                    query = query.Where(x => categories.Contains(x.category_id));
                }
                if (auction.pagination_direction_type == PaginationDirectionType.Backward)
                {
                    auction.order_type = auction.order_type == OrderType.Descending ? OrderType.Ascending : OrderType.Descending;
                }
                query = auction.order_type == OrderType.Ascending
                    ? query.OrderBy(x => x.ID)
                    : query.OrderByDescending(x => x.ID);
                query = query
                    .Skip(auction.rows_to_skip)
                    .Take(10)
                    .Include(x => x.Bids)
                    .Include(x => x.Images)
                    .Include(x => x.User)
                    .Include(x => x.User!.ReviewsReceived);
                db_resp = await query.ToListAsync();
            }
            /*
                SortType.AlmostFinished - Veoma slična logika kao SortType.Price - Pročitaj donji komentar. 
            */
            else if (auction.sort_type == SortType.AlmostFinished)
            {
                if (auction.pagination_direction_type == PaginationDirectionType.Backward)
                {
                    auction.order_type = auction.order_type == OrderType.Descending ? OrderType.Ascending : OrderType.Descending;
                }
                var query = _context.Auctions.AsNoTracking();
                if (auction.date_of_expiration_filter.HasValue) {
                    query = auction.order_type == OrderType.Ascending
                        ? query.Where(x => x.date_of_expiration >= auction.date_of_expiration_filter)
                        : query.Where(x => x.date_of_expiration <= auction.date_of_expiration_filter);
                    if (auction.auction_id_filter != 0)
                    {
                        query = query.Where(x => (x.ID > auction.auction_id_filter && x.date_of_expiration == auction.date_of_expiration_filter) || x.date_of_expiration != auction.date_of_expiration_filter);
                    }
                }
                if (auction.category_id != 0)
                {
                    query = query.Where(x => categories.Contains(x.category_id));
                }
                if (auction.pagination_direction_type == PaginationDirectionType.Backward)
                {
                    auction.order_type = auction.order_type == OrderType.Descending ? OrderType.Ascending : OrderType.Descending;
                }
                query = auction.order_type == OrderType.Ascending
                    ? query.OrderBy(x => x.date_of_expiration).ThenBy(x => x.ID)
                    : query.OrderByDescending(x => x.date_of_expiration).ThenBy(x => x.ID);
                query = query
                    .Skip(auction.rows_to_skip)
                    .Take(10)
                    .Include(x => x.Bids)
                    .Include(x => x.Images)
                    .Include(x => x.User)
                    .Include(x => x.User!.ReviewsReceived);
                db_resp = await query.ToListAsync();
            }
            /*
                SortType.Price - Logika iza ovog sorta je složena.
                Za prvo vraćanje uvek uzimamo prvih 10 redova orderovanih po željenom redosledu.
                Za ostalo pošto je ovde sort po onome što nije unikatno moramo da šaljemo i auction id i cenu poslednjeg vraćenog podatka iz prošlog sortiranja.
                Zašto ThenBy(x => x.ID) - Ako imamo 6 proizvoda koji koštaju 1500 moramo da osiguramo da će nakon 1500 sledeći sort da bude po ID odnosno da će se prvo prikazati oni od 1500 koji imaju manji ID, zašto?
                Ako je poslata cena i auction_id kada taj auction_id koji je predhodno vraćen ne bi bio sortiran u redosledu mi ne bismo mogli da znamo koji od tih proizvoda sa istom cenom smo već vratili a ovako jednostavno kažemo daj mi sve što je veće od tog ID ako je cena ista kao cena koju smo poslali.
            */
            else
            {
                if (auction.pagination_direction_type == PaginationDirectionType.Backward)
                {
                    auction.order_type = auction.order_type == OrderType.Descending ? OrderType.Ascending : OrderType.Descending;
                }
                var query = _context.Auctions.AsNoTracking();
                if(auction.price_filter != 0)
                {
                    query = auction.order_type == OrderType.Ascending
                        ? query.Where(x => x.startPrice >= auction.price_filter)
                        : query.Where(x => x.startPrice <= auction.price_filter);
                    if(auction.auction_id_filter != 0)
                    {
                        query = query.Where(x => (x.ID > auction.auction_id_filter && x.startPrice == auction.price_filter) || x.startPrice != auction.price_filter);
                    }
                }
                if (auction.category_id != 0)
                {
                    query = query.Where(x => categories.Contains(x.category_id));
                }
                if (auction.pagination_direction_type == PaginationDirectionType.Backward)
                {
                    auction.order_type = auction.order_type == OrderType.Descending ? OrderType.Ascending : OrderType.Descending;
                }
                query = auction.order_type == OrderType.Ascending
                    ? query.OrderBy(x => x.startPrice).ThenBy(x => x.ID)
                    : query.OrderByDescending(x => x.startPrice).ThenBy(x => x.ID);
                query = query
                    .Skip(auction.rows_to_skip)
                    .Take(10)
                    .Include(x => x.Bids)
                    .Include(x => x.Images)
                    .Include(x => x.User)
                    .Include(x => x.User!.ReviewsReceived);
                db_resp = await query.ToListAsync();
            }
            response.auctions = db_resp.Select(x => new AuctionsDataDto
            {
                ID = x.ID,
                username = x.User!.username,
                average_rating = x.User!.ReviewsReceived.Count > 0 ? x.User.ReviewsReceived.Average(x => x.rating) : -1,
                total_reviews = x.User!.ReviewsReceived.Count,
                category_id = x.category_id,
                title = x.title,
                description = x.description,
                startPrice = x.startPrice,
                date_of_auction = x.date_of_auction,
                date_of_expiration = x.date_of_expiration,
                images = x.Images.Select(x => x.ToDetailsDto()).ToList(),
                top_bid = x.Bids.Count > 0 ? x.Bids.Where(bid => bid.amount == x.Bids.Max(b => b.amount)).FirstOrDefault()!.ToDetailsDto(): null,
            }).ToList();
            return response;
        }
        public async Task<bool> ValidateImages(List<string> images)
        {
            return await Task.Run(() =>
            {
                try
                {
                    foreach (string image in images)
                    {
                        string base64Data = image.Replace("data:application/octet-stream;base64,", "");
                        byte[] imageBytes = Convert.FromBase64String(base64Data);
                        using (SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(imageBytes))
                        {
                            if (!(img.Metadata.DecodedImageFormat is JpegFormat))
                            {
                                return false;
                            }
                            if(img.Width != 500 || img.Height != 500) //Slike su striktno 500x500
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }
    }
}
