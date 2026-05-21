using Domain.Entities;
using Domain.Entities.Enums;
using ChatEntity = Domain.Entities.Chat;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public  DbSet<ChatEntity> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatParticipant> Participants { get; set; }
        DbSet<GroupMessageStatus> MessageStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(user =>
            {
                user.HasKey(e => e.Id);
                user.Property(e => e.Id)
                                    .HasDefaultValueSql("gen_random_uuid()");

                user.Property(e => e.PasswordHash).IsRequired();

                user.Property(e => e.Name)
                                    .IsRequired()
                                    .HasMaxLength(256);

                user.Property(e => e.Email) 
                                    .IsRequired()
                                    .HasMaxLength(256);

                user.HasIndex(e => e.Email).IsUnique();

                user.Property(e => e.Role)
                                    .HasConversion<string>()
                                    .HasMaxLength(20);
            });

            modelBuilder.Entity<ChatEntity>(chat =>
            {
                chat.HasKey(e => e.Id);
                chat.Property(e => e.Id)
                                    .HasDefaultValueSql("gen_random_uuid()");

                chat.Property(e => e.Name)
                                    .IsRequired()
                                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Message>(message =>
            {
                message.HasKey(e => e.Id);  

                message.Property(e => e.Id)
                            .HasDefaultValueSql("gen_random_uuid()");

                message.Property(e => e.Content).IsRequired();

                message.Property(e => e.Timestamp)
                                            .HasDefaultValueSql("now()");

                message.Property(e => e.Status)
                                        .IsRequired()
                                        .HasDefaultValue(MessageStatus.Delivered)
                                        .HasConversion<string>();

                message.HasOne(e => e.User)
                                .WithMany(u => u.Messages)
                                .HasForeignKey(u => u.UserId);

                message.HasOne(e => e.Chat)
                                        .WithMany(e => e.Messages)
                                        .HasForeignKey(e => e.ChatId);
            });

            modelBuilder.Entity<ChatParticipant>(chParticipant =>
            {
                chParticipant.HasKey(e => new { e.UserId, e.ChatId });

                chParticipant.Property(e => e.JoinedAt)
                                            .HasDefaultValueSql("now()");

                chParticipant.HasOne(e => e.User)
                                            .WithMany(u => u.Participants)
                                            .HasForeignKey(m => m.UserId);

                chParticipant.HasOne(e => e.Chat)
                                              .WithMany(e => e.Participants)
                                              .HasForeignKey(e => e.ChatId);

                chParticipant.Property(e => e.Role)
                                               .HasConversion<string>()
                                               .HasMaxLength(20);
            });

            modelBuilder.Entity<GroupMessageStatus>()
           .HasIndex(s => new { s.MessageId, s.RecipientId });
        }
    }
}
