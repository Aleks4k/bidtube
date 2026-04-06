using bidtube.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Domain.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}
        public DbSet<User> Users { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<AuctionImage> AuctionImages { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }
        public DbSet<AuctionHistory> Auctions_history { get; set; }
        public DbSet<BidHistory> Bids_history { get; set; }
        public DbSet<AuctionImageHistory> AuctionImages_history { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                    .HasColumnName("user_id")
                    .HasColumnType("int")
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.username)
                    .HasColumnType("varchar(24)")
                    .IsRequired();
                entity.Property(e => e.password)
                    .HasColumnType("binary(60)")
                    .IsRequired();
                entity.Property(e => e.mail)
                    .HasColumnType("varchar(255)")
                    .IsRequired();
                entity.Property(e => e.date_of_birth)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .IsRequired(false);
                entity.Property(e => e.phone)
                    .HasColumnType("varchar(25)")
                    .IsRequired(false);
                entity.Property(e => e.msg_allowed_from_anyone)
                    .HasColumnName("msg_allowed_from")
                    .HasColumnType("bit")
                    .IsRequired();
            });
            modelBuilder.Entity<Auction>(entity =>
            {
                entity.ToTable("auction");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                    .HasColumnName("auction_id")
                    .HasColumnType("int")
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.user_id)
                    .HasColumnType("int")
                    .IsRequired();
                entity.Property(e => e.category_id)
                    .HasColumnType("int")
                    .IsRequired();
                entity.Property(e => e.title)
                    .HasColumnType("varchar(64)")
                    .IsRequired();
                entity.Property(e => e.description)
                    .HasColumnType("varchar(1024)")
                    .IsRequired();
                entity.Property(e => e.startPrice)
                    .HasColumnName("start_price")
                    .HasColumnType("decimal(20, 3)")
                    .IsRequired();
                entity.Property(e => e.date_of_auction)
                    .HasColumnType("timestamp")
                    .IsRequired()
                    .HasDefaultValueSql("current_timestamp");
                entity.Property(e => e.date_of_expiration)
                    .HasColumnType("timestamp")
                    .IsRequired()
                    .HasDefaultValueSql("current_timestamp");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Auctions)
                    .HasForeignKey(e => e.user_id)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Auctions)
                    .HasForeignKey(e => e.category_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<AuctionHistory>(entity =>
            {
                entity.ToTable("auction_history");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                    .HasColumnName("auction_history_id")
                    .HasColumnType("int")
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.auction_id)
                    .HasColumnType("int")
                    .IsRequired();
                entity.Property(e => e.user_id)
                    .HasColumnType("int")
                    .IsRequired();
                entity.Property(e => e.category_id)
                    .HasColumnType("int")
                    .IsRequired();
                entity.Property(e => e.user_bought_id)
                    .HasColumnType("int");
                entity.Property(e => e.title)
                    .HasColumnType("varchar(64)")
                    .IsRequired();
                entity.Property(e => e.description)
                    .HasColumnType("varchar(1024)")
                    .IsRequired();
                entity.Property(e => e.startPrice)
                    .HasColumnName("start_price")
                    .HasColumnType("decimal(20, 3)")
                    .IsRequired();
                entity.Property(e => e.date_of_auction)
                    .HasColumnType("timestamp")
                    .IsRequired()
                    .HasDefaultValueSql("current_timestamp");
                entity.Property(e => e.date_of_expiration)
                    .HasColumnType("timestamp")
                    .IsRequired()
                    .HasDefaultValueSql("current_timestamp");
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Auctions_history)
                    .HasForeignKey(e => e.category_id)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Winner)
                    .WithMany(c => c.AuctionsWon)
                    .HasForeignKey(e => e.user_bought_id)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.User)
                    .WithMany(c => c.Auctions_history)
                    .HasForeignKey(e => e.user_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<AuctionImage>(entity =>
            {
                entity.ToTable("auction_image");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                    .HasColumnName("auction_image_id")
                    .HasColumnType("int")
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.auction_id)
                   .HasColumnType("int")
                   .IsRequired();
                entity.Property(e => e.url)
                    .HasColumnName("image_url")
                    .HasColumnType("varchar(255)")
                    .IsRequired();
                entity.Property(e => e.alt_text)
                    .HasColumnType("varchar(96)")
                    .IsRequired();
                entity.HasOne(e => e.Auction)
                    .WithMany(a => a.Images)
                    .HasForeignKey(e => e.auction_id)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<AuctionImageHistory>(entity =>
            {
                entity.ToTable("auction_image_history");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                    .HasColumnName("auction_image_history_id")
                    .HasColumnType("int")
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.auction_image_id)
                   .HasColumnType("int")
                   .IsRequired();
                entity.Property(e => e.auction_id)
                   .HasColumnType("int")
                   .IsRequired();
                entity.Property(e => e.auction_history_id)
                   .HasColumnType("int")
                   .IsRequired();
                entity.Property(e => e.url)
                    .HasColumnName("image_url")
                    .HasColumnType("varchar(255)")
                    .IsRequired();
                entity.Property(e => e.alt_text)
                    .HasColumnType("varchar(96)")
                    .IsRequired();
                entity.HasOne(e => e.Auction)
                    .WithMany(a => a.Images)
                    .HasForeignKey(e => e.auction_history_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Bid>(entity =>
            {
                entity.ToTable("bid");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                    .HasColumnName("bid_id")
                    .HasColumnType("int")
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.user_id)
                   .HasColumnType("int")
                   .IsRequired();
                entity.Property(e => e.auction_id)
                   .HasColumnType("int")
                   .IsRequired();
                entity.Property(e => e.amount)
                    .HasColumnType("decimal(20, 3)")
                    .IsRequired();
                entity.Property(e => e.date_of_bid)
                    .HasColumnType("timestamp")
                    .IsRequired()
                    .HasDefaultValueSql("current_timestamp");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserBids)
                    .HasForeignKey(e => e.user_id)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Auction)
                    .WithMany(a => a.Bids)
                    .HasForeignKey(e => e.auction_id)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<BidHistory>(entity =>
            {
                entity.ToTable("bid_history");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                    .HasColumnName("bid_history_id")
                    .HasColumnType("int")
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.bid_id)
                  .HasColumnType("int")
                  .IsRequired();
                entity.Property(e => e.user_id)
                   .HasColumnType("int")
                   .IsRequired();
                entity.Property(e => e.auction_id)
                   .HasColumnType("int")
                   .IsRequired();
                entity.Property(e => e.auction_history_id)
                   .HasColumnType("int")
                   .IsRequired();
                entity.Property(e => e.amount)
                    .HasColumnType("decimal(20, 3)")
                    .IsRequired();
                entity.Property(e => e.date_of_bid)
                    .HasColumnType("timestamp")
                    .IsRequired()
                    .HasDefaultValueSql("current_timestamp");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserBids_history)
                    .HasForeignKey(e => e.user_id)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Auction)
                    .WithMany(a => a.Bids)
                    .HasForeignKey(e => e.auction_history_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                    .HasColumnName("category_id")
                    .HasColumnType("int")
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.parent_category_id)
                    .HasColumnType("int")
                    .IsRequired(false);
                entity.Property(e => e.name)
                    .HasColumnType("varchar(64)")
                    .IsRequired();
                entity.Property(e => e.icon_name)
                   .HasColumnType("varchar(64)");
                entity.HasOne(e => e.Parent)
                    .WithMany(a => a.Subcategories)
                    .HasForeignKey(e => e.parent_category_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("review");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                    .HasColumnName("review_id")
                    .HasColumnType("int")
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.received_id)
                    .HasColumnType("int")
                    .IsRequired();
                entity.Property(e => e.sent_id)
                    .HasColumnType("int")
                    .IsRequired();
                entity.Property(e => e.auction_history_id)
                    .HasColumnType("int")
                    .IsRequired();
                entity.Property(e => e.rating)
                    .HasColumnType("tinyint")
                    .IsRequired();
                entity.Property(e => e.comment)
                    .HasColumnType("varchar(255)");
                entity.Property(e => e.date_of_review)
                    .HasColumnType("timestamp")
                    .IsRequired()
                    .HasDefaultValueSql("current_timestamp");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.ReviewsReceived)
                    .HasForeignKey(e => e.received_id)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.User_Sent)
                    .WithMany(u => u.ReviewsGiven)
                    .HasForeignKey(e => e.sent_id)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Auction)
                    .WithMany(a => a.Reviews)
                    .HasForeignKey(e => e.auction_history_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<EventLog>(entity =>
            {
                entity.ToTable("event_log");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                    .HasColumnName("event_id")
                    .HasColumnType("int")
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.error_code)
                    .HasColumnType("int");
                entity.Property(e => e.error_message)
                    .HasColumnType("varchar(64)");
                entity.Property(e => e.error_timestamp)
                    .HasColumnType("timestamp")
                    .IsRequired()
                    .HasDefaultValueSql("current_timestamp");
            });
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notification");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID)
                    .HasColumnName("notification_id")
                    .HasColumnType("int")
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.user_id)
                    .HasColumnType("int")
                    .IsRequired();
                entity.Property(e => e.title)
                    .HasColumnType("varchar(64)")
                    .IsRequired();
                entity.Property(e => e.description)
                    .HasColumnType("varchar(1024)")
                    .IsRequired();
                entity.Property(e => e.action)
                    .HasColumnType("varchar(15)");
                entity.Property(e => e.status)
                    .HasColumnType("tinyint")
                    .IsRequired();
                entity.Property(e => e.date_of_notification)
                    .HasColumnType("timestamp")
                    .IsRequired()
                    .HasDefaultValueSql("current_timestamp");
                entity.Property(e => e.date_of_read)
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("current_timestamp");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.user_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
