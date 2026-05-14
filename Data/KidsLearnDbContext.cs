using KidsLearn.Models;
using Microsoft.EntityFrameworkCore;

namespace KidsLearn.Data;

public partial class KidsLearnDbContext : DbContext
{
    public KidsLearnDbContext()
    {
    }

    public KidsLearnDbContext(DbContextOptions<KidsLearnDbContext> options)
        : base(options)
    {
    }

    // =========================================================
    // DB SETS - 12 BẢNG (7 cũ + 5 mới)
    // =========================================================

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Grade> Grades { get; set; }
    public virtual DbSet<Unit> Units { get; set; }
    public virtual DbSet<Vocabulary> Vocabularies { get; set; }
    public virtual DbSet<Quiz> Quizzes { get; set; }
    public virtual DbSet<Answer> Answers { get; set; }
    public virtual DbSet<LearningProgress> LearningProgresses { get; set; }

    // ✨ MỚI: 5 DbSet cho 5 bảng mới
    public virtual DbSet<QuizAttemptDetail> QuizAttemptDetails { get; set; }
    public virtual DbSet<UserStreak> UserStreaks { get; set; }
    public virtual DbSet<UserActivity> UserActivities { get; set; }
    public virtual DbSet<Badge> Badges { get; set; }
    public virtual DbSet<UserBadge> UserBadges { get; set; }

    // =========================================================
    // MODEL CONFIGURATION (Fluent API)
    // =========================================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // =====================================================
        // USER
        // =====================================================
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.ToTable("Users");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.AvatarUrl).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            // ✨ MỚI: 2 cột bổ sung
            entity.Property(e => e.TotalXP).HasDefaultValue(0);
            entity.Property(e => e.Level).HasDefaultValue(1).HasColumnName("Level");
        });

        // =====================================================
        // GRADE
        // =====================================================
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId);
            entity.ToTable("Grade");
            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.GradeName).HasMaxLength(50);
        });

        // =====================================================
        // UNIT
        // =====================================================
        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.UnitId);
            entity.ToTable("Unit");
            entity.Property(e => e.UnitId).HasColumnName("UnitID");
            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Grade)
                .WithMany(p => p.Units)
                .HasForeignKey(d => d.GradeId)
                .HasConstraintName("FK_Unit_Grade");
        });

        // =====================================================
        // VOCABULARY
        // =====================================================
        modelBuilder.Entity<Vocabulary>(entity =>
        {
            entity.HasKey(e => e.VocabId);
            entity.ToTable("Vocabulary");
            entity.Property(e => e.VocabId).HasColumnName("VocabID");
            entity.Property(e => e.UnitId).HasColumnName("UnitID");
            entity.Property(e => e.Word).HasMaxLength(100);
            entity.Property(e => e.Mean).HasMaxLength(255);
            entity.Property(e => e.Ipa).HasColumnName("IPA").HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Example).HasMaxLength(255);
            entity.Property(e => e.TtsText).HasColumnName("TTS_Text").HasMaxLength(255);

            entity.HasOne(d => d.Unit)
                .WithMany(p => p.Vocabularies)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK_Vocabulary_Unit");
        });

        // =====================================================
        // QUIZ
        // =====================================================
        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(e => e.QuizId);
            entity.ToTable("Quiz");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.UnitId).HasColumnName("UnitID");
            entity.Property(e => e.QuestionText).HasMaxLength(255);
            entity.Property(e => e.QuestionType).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.TtsText).HasColumnName("TTS_Text").HasMaxLength(255);

            entity.HasOne(d => d.Unit)
                .WithMany(p => p.Quizzes)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK_Quiz_Unit");
        });

        // =====================================================
        // ANSWER
        // =====================================================
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.AnswerId);
            entity.ToTable("Answer");
            entity.Property(e => e.AnswerId).HasColumnName("AnswerID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.AnswerText).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Quiz)
                .WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuizId)
                .HasConstraintName("FK_Answer_Quiz");
        });

        // =====================================================
        // LEARNING PROGRESS
        // =====================================================
        modelBuilder.Entity<LearningProgress>(entity =>
        {
            entity.HasKey(e => e.ProgressId);
            entity.ToTable("LearningProgress");
            entity.Property(e => e.ProgressId).HasColumnName("ProgressID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UnitId).HasColumnName("UnitID");
            entity.Property(e => e.Score).HasColumnType("decimal(5,2)");
            entity.Property(e => e.CompletedAt).HasColumnType("datetime");

            entity.HasOne(d => d.User)
                .WithMany(p => p.LearningProgresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Progress_User");

            entity.HasOne(d => d.Unit)
                .WithMany(p => p.LearningProgresses)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK_Progress_Unit");
        });

        // =====================================================
        // ✨ MỚI: QUIZ ATTEMPT DETAIL
        // =====================================================
        modelBuilder.Entity<QuizAttemptDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId);
            entity.ToTable("QuizAttemptDetail");
            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.ProgressId).HasColumnName("ProgressID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.SelectedAnswerId).HasColumnName("SelectedAnswerID");

            // Khi xóa LearningProgress → tự xóa luôn QuizAttemptDetail (CASCADE)
            entity.HasOne(d => d.Progress)
                .WithMany(p => p.QuizAttemptDetails)
                .HasForeignKey(d => d.ProgressId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_QuizAttemptDetail_LearningProgress");

            // QUAN TRỌNG: NO ACTION cho Quiz và Answer để tránh multiple cascade paths
            // SQL Server không cho phép 1 record bị xóa qua nhiều đường CASCADE
            entity.HasOne(d => d.Quiz)
                .WithMany(p => p.QuizAttemptDetails)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_QuizAttemptDetail_Quiz");

            entity.HasOne(d => d.SelectedAnswer)
                .WithMany(p => p.QuizAttemptDetails)
                .HasForeignKey(d => d.SelectedAnswerId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_QuizAttemptDetail_Answer");
        });

        // =====================================================
        // ✨ MỚI: USER STREAK
        // =====================================================
        modelBuilder.Entity<UserStreak>(entity =>
        {
            entity.HasKey(e => e.StreakId);
            entity.ToTable("UserStreak");
            entity.Property(e => e.StreakId).HasColumnName("StreakID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.LastStudyDate).HasColumnType("date");

            // UNIQUE: mỗi user chỉ có 1 record streak
            entity.HasIndex(e => e.UserId).IsUnique();

            entity.HasOne(d => d.User)
                .WithOne(p => p.UserStreak)
                .HasForeignKey<UserStreak>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserStreak_Users");
        });

        // =====================================================
        // ✨ MỚI: USER ACTIVITY
        // =====================================================
        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.HasKey(e => e.ActivityId);
            entity.ToTable("UserActivity");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UnitId).HasColumnName("UnitID");
            entity.Property(e => e.LastAccessedAt).HasColumnType("datetime");

            // UNIQUE: mỗi user có 1 record cho mỗi Unit
            entity.HasIndex(e => new { e.UserId, e.UnitId }).IsUnique();

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserActivities)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserActivity_Users");

            entity.HasOne(d => d.Unit)
                .WithMany(p => p.UserActivities)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserActivity_Unit");
        });

        // =====================================================
        // ✨ MỚI: BADGE
        // =====================================================
        modelBuilder.Entity<Badge>(entity =>
        {
            entity.HasKey(e => e.BadgeId);
            entity.ToTable("Badge");
            entity.Property(e => e.BadgeId).HasColumnName("BadgeID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IconUrl).HasMaxLength(255);
            entity.Property(e => e.ConditionType).HasMaxLength(255);
        });

        // =====================================================
        // ✨ MỚI: USER BADGE (bảng quan hệ N-N)
        // =====================================================
        modelBuilder.Entity<UserBadge>(entity =>
        {
            entity.HasKey(e => e.UserBadgeId);
            entity.ToTable("UserBadge");
            entity.Property(e => e.UserBadgeId).HasColumnName("UserBadgeID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.BadgeId).HasColumnName("BadgeID");
            entity.Property(e => e.EarnedAt).HasColumnType("datetime");

            // UNIQUE: 1 user chỉ nhận 1 huy hiệu 1 lần
            entity.HasIndex(e => new { e.UserId, e.BadgeId }).IsUnique();

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserBadges)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserBadge_Users");

            entity.HasOne(d => d.Badge)
                .WithMany(p => p.UserBadges)
                .HasForeignKey(d => d.BadgeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserBadge_Badge");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}