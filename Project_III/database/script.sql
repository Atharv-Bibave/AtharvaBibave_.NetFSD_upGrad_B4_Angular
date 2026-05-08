CREATE DATABASE EventManagementSystemDB;
USE EventManagementSystemDB
GO

/****** Create Tables ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- EF Migrations history table
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
) ON [PRIMARY]
GO

-- Categories (must be created before Events due to FK)
CREATE TABLE [dbo].[Categories](
	[Id] [uniqueidentifier] NOT NULL,
	[CategoryName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_Categories_Name] ON [dbo].[Categories]([CategoryName] ASC)
GO

-- Speakers
CREATE TABLE [dbo].[Speakers](
	[SpeakerId] [uniqueidentifier] NOT NULL,
	[SpeakerName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Speakers] PRIMARY KEY CLUSTERED ([SpeakerId] ASC)
) ON [PRIMARY]
GO

-- Users
CREATE TABLE [dbo].[Users](
	[EmailId] [nvarchar](256) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Role] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([EmailId] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- Events (FK -> Categories)
CREATE TABLE [dbo].[Events](
	[EventId] [uniqueidentifier] NOT NULL,
	[EventName] [nvarchar](50) NOT NULL,
	[EventCategory] [nvarchar](50) NOT NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[EventDate] [datetime2](7) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Status] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED ([EventId] ASC)
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Events_CategoryId] ON [dbo].[Events]([CategoryId] ASC)
GO

ALTER TABLE [dbo].[Events] WITH CHECK ADD CONSTRAINT [FK_Events_Categories_CategoryId]
FOREIGN KEY([CategoryId]) REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_Categories_CategoryId]
GO

-- Sessions (FK -> Events, Speakers)
CREATE TABLE [dbo].[Sessions](
	[SessionId] [uniqueidentifier] NOT NULL,
	[EventId] [uniqueidentifier] NOT NULL,
	[SessionTitle] [nvarchar](50) NOT NULL,
	[SpeakerId] [uniqueidentifier] NULL,
	[Description] [nvarchar](500) NULL,
	[SessionStart] [datetime2](7) NOT NULL,
	[SessionEnd] [datetime2](7) NOT NULL,
	[SessionUrl] [nvarchar](2048) NULL,
 CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED ([SessionId] ASC)
) ON [PRIMARY]
GO

-- ParticipantEvents (FK -> Users, Events)
CREATE TABLE [dbo].[ParticipantEvents](
	[Id] [uniqueidentifier] NOT NULL,
	[ParticipantEmailId] [nvarchar](256) NOT NULL,
	[EventId] [uniqueidentifier] NOT NULL,
	[IsAttended] [bit] NOT NULL,
 CONSTRAINT [PK_ParticipantEvents] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

/****** Create Indexes ******/
CREATE NONCLUSTERED INDEX [IX_Sessions_EventId] ON [dbo].[Sessions]([EventId] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_Sessions_SpeakerId] ON [dbo].[Sessions]([SpeakerId] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_ParticipantEvents_EventId] ON [dbo].[ParticipantEvents]([EventId] ASC)
GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_ParticipantEvents_Email_Event] ON [dbo].[ParticipantEvents]
([ParticipantEmailId] ASC, [EventId] ASC)
GO

/****** Add Foreign Keys ******/
ALTER TABLE [dbo].[Sessions] WITH CHECK ADD CONSTRAINT [FK_Sessions_Events_EventId]
FOREIGN KEY([EventId]) REFERENCES [dbo].[Events] ([EventId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Sessions] CHECK CONSTRAINT [FK_Sessions_Events_EventId]
GO

ALTER TABLE [dbo].[Sessions] WITH CHECK ADD CONSTRAINT [FK_Sessions_Speakers_SpeakerId]
FOREIGN KEY([SpeakerId]) REFERENCES [dbo].[Speakers] ([SpeakerId]) ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Sessions] CHECK CONSTRAINT [FK_Sessions_Speakers_SpeakerId]
GO

ALTER TABLE [dbo].[ParticipantEvents] WITH CHECK ADD CONSTRAINT [FK_ParticipantEvents_Events_EventId]
FOREIGN KEY([EventId]) REFERENCES [dbo].[Events] ([EventId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ParticipantEvents] CHECK CONSTRAINT [FK_ParticipantEvents_Events_EventId]
GO

ALTER TABLE [dbo].[ParticipantEvents] WITH CHECK ADD CONSTRAINT [FK_ParticipantEvents_Users_ParticipantEmailId]
FOREIGN KEY([ParticipantEmailId]) REFERENCES [dbo].[Users] ([EmailId]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ParticipantEvents] CHECK CONSTRAINT [FK_ParticipantEvents_Users_ParticipantEmailId]
GO

/****** Insert Data ******/

-- Mark migration as applied (single clean migration)
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260402000000_InitialCreate', N'8.0.11')
GO

-- Seed Categories (5 standard categories)
INSERT [dbo].[Categories] ([Id], [CategoryName]) VALUES
(N'11111111-0000-0000-0000-000000000001', N'Tech & Innovation'),
(N'11111111-0000-0000-0000-000000000002', N'Industrial Event'),
(N'11111111-0000-0000-0000-000000000003', N'Workshop'),
(N'11111111-0000-0000-0000-000000000004', N'Solution and Projects'),
(N'11111111-0000-0000-0000-000000000005', N'EMS (Solution)')
GO

-- Seed Speakers
INSERT [dbo].[Speakers] ([SpeakerId], [SpeakerName]) VALUES
(N'e20247f0-6521-4194-abb9-23589ea0ee31', N'Anand Tiwari'),
(N'4b048311-fea3-4638-b960-2f4ac1b9d725', N'Ankit Verma'),
(N'c0015731-3283-4b51-9e13-391f3fda7f13', N'Kavita Singh'),
(N'a0d28d9b-6d1e-4742-885b-53d89184557a', N'Amit Desai'),
(N'50c12f15-087d-4261-926f-5f56088ecd11', N'Dr. Sneha Joshi'),
(N'14844750-01bb-4d74-9cce-76d821705cad', N'Neha Kulkarni'),
(N'b1685fbe-8d78-40ba-8d44-af9e1956eac0', N'Priya Mehta'),
(N'fe77fd80-16c1-4464-9371-c0a0864af060', N'Dr. Rajesh Sharma'),
(N'9e1a09f6-ed0c-4b9e-ac3f-c16b1dacd866', N'Rahul Patil'),
(N'e6f1047d-03df-4690-8517-c659182f9a77', N'Arjun Nair')
GO

-- Seed Users (Admin + Participants)
INSERT [dbo].[Users] ([EmailId], [UserName], [Role], [Password]) VALUES
(N'admin@ems.com',                    N'Admin',          N'Admin',       N'$2a$11$IAF00m.BFFGKVKi.eFn3YO43CJ7box7Gm2S8LOO6UWRVlpzKLUWa2'),
(N'adityashinde.developer@gmail.com', N'Aditya Shinde',  N'Participant', N'$2a$11$LhtathOUUHhziJe6w3.nX.Kl0IXu7tW9uWRqwQMKpMwOnSAnbVrm6'),
(N'ankitdudhe.codes@gmail.com',       N'Ankit Dudhe',    N'Participant', N'$2a$11$9.D1iLAOr9CiJF9qEj7IXO3hde7HDNuUql/.KD3eo96erZtJV9Z.i'),
(N'atharvabibave@gmail.com',          N'Atharva Bibave', N'Participant', N'$2a$11$rjsDIF.sRZN7JT.BG6m1ReyvKaZ6qeDgWwXmTzGOj6K70WrsSKj6y'),
(N'priyamehta.tech@gmail.com',        N'Priya Mehta',    N'Participant', N'$2a$11$9pijtBFKJhboWu2x0IsuwO3YwLKCIAIQBaidjEwOneVhXWfXof5k6'),
(N'rohitsharma.dev@gmail.com',        N'Rohit Sharma',   N'Participant', N'$2a$11$7BUZpepzOb0AhQY9hBbqZuWmKr8wCq9fTVzPVwW.olU6CWhNuxq7e'),
(N'shivamboradhe@gmail.com',          N'Shivam Boradhe', N'Participant', N'$2a$11$Kj4lvO8tLT.zIc5t9GgWaONBNQeUTFcZUjgFLB2pNUufPlkLIjOyu'),
(N'shritejnagre.coder@gmail.com',     N'Shritej Nagre',  N'Participant', N'$2a$11$CazFwqPJkwTvBwdLmqUcSewBICaGq5/fyOIX7oysWx7n/fIIN4vn6'),
(N'shubhamgaikwad@gmail.com',         N'Shubham Gaikwad',N'Participant', N'$2a$11$vc2TxWMEYkWW5iy35VS4/ONLCb2Srtm.tabj6slyktn1IZeLk5T1S'),
(N'tejasmote.intern@gmail.com',       N'Tejas Mote',     N'Participant', N'$2a$11$D.RXy09eRKDS1qXRHUJLH.pgmw5BegreoxHiAkzJ3EVC3qrWKtpWG'),
(N'umeshunde09@gmail.com',            N'Umesh Unde',     N'Participant', N'$2a$11$S.ea6sk66jmlAXWub7wLgu1BVauuFrlSUTFGjU1S9BzBUXvYL65ka'),
(N'yogeshbarure9@gmail.com',          N'Yogesh Barure',  N'Participant', N'$2a$11$Of4hcn5Ia2KbvpNWeR5CF.rYsZJYH3B/cZqwRAivhevHgzYuEfnNe')
GO

-- Seed Events (CategoryId mapped to the 5 seed categories)
-- Note: Events below use custom category names not in the 5 seed categories.
-- A new category 'Other' is added to handle these gracefully.
INSERT [dbo].[Categories] ([Id], [CategoryName]) VALUES
(N'22222222-0000-0000-0000-000000000001', N'Tech & Innovations'),
(N'22222222-0000-0000-0000-000000000002', N'Business & Entrepreneurship'),
(N'22222222-0000-0000-0000-000000000003', N'Health & Wellness'),
(N'22222222-0000-0000-0000-000000000004', N'Education & Research'),
(N'22222222-0000-0000-0000-000000000005', N'Industrial Events')
GO

INSERT [dbo].[Events] ([EventId], [EventName], [EventCategory], [CategoryId], [EventDate], [Description], [Status]) VALUES
(N'e0c42a82-146c-4b09-8af5-b7b3a1fe2804', N'AI Future Summit 2026',         N'Tech & Innovations',           N'22222222-0000-0000-0000-000000000001', CAST(N'2026-04-22T00:00:00.0000000' AS DateTime2), N'A conference focused on AI trends, machine learning, and real-world applications.',            N'Active'),
(N'73e35241-1d0d-44db-9d64-7d2bf67e81e0', N'Entrepreneurship Growth Summit', N'Business & Entrepreneurship',  N'22222222-0000-0000-0000-000000000002', CAST(N'2026-04-23T00:00:00.0000000' AS DateTime2), N'Learn strategies for scaling startups and business growth.',                                  N'Active'),
(N'2295c20c-0163-42fd-864b-ce0e2fcde1bd', N'Global Wellness Retreat',        N'Health & Wellness',            N'22222222-0000-0000-0000-000000000003', CAST(N'2026-04-24T00:00:00.0000000' AS DateTime2), N'Focus on mental health, relaxation, and holistic well-being practices.',                      N'Active'),
(N'978719fa-cd5a-4987-8714-449c3c4fffaa', N'Smart Manufacturing Expo',       N'Industrial Events',            N'22222222-0000-0000-0000-000000000005', CAST(N'2026-04-25T00:00:00.0000000' AS DateTime2), N'Showcase of advanced manufacturing technologies and automation solutions.',                    N'Active'),
(N'a38141c8-fd03-41a3-8658-14c80d6785e1', N'International Education Fair',   N'Education & Research',         N'22222222-0000-0000-0000-000000000004', CAST(N'2026-04-26T00:00:00.0000000' AS DateTime2), N'Meet universities and explore higher education opportunities worldwide.',                      N'Active'),
(N'5a820b41-ebb7-49ec-a0e1-e900f43017e6', N'Blockchain & Web3 Expo',         N'Tech & Innovations',           N'22222222-0000-0000-0000-000000000001', CAST(N'2026-05-05T00:00:00.0000000' AS DateTime2), N'Explore blockchain technology, crypto innovations, and decentralized apps.',                   N'Active'),
(N'82d0d7b2-3e3f-401e-99b1-ae993fb02d5d', N'Startup Funding & VC Meet',      N'Business & Entrepreneurship',  N'22222222-0000-0000-0000-000000000002', CAST(N'2026-05-08T00:00:00.0000000' AS DateTime2), N'Connect startups with investors and venture capitalists.',                                    N'Active'),
(N'714be219-3528-4e59-be50-ce2eee89e7cb', N'Automation & Robotics Show',     N'Industrial Events',            N'22222222-0000-0000-0000-000000000005', CAST(N'2026-05-10T00:00:00.0000000' AS DateTime2), N'Exhibition of robotics innovations and industrial automation systems.',                        N'Active'),
(N'8a5ffc9f-bdc1-448c-9c9d-795e589606c0', N'Yoga & Meditation Camp',         N'Health & Wellness',            N'22222222-0000-0000-0000-000000000003', CAST(N'2026-05-22T00:00:00.0000000' AS DateTime2), N'Guided yoga sessions and meditation workshops for stress relief.',                            N'Active'),
(N'5ba8d7f9-334e-4e8f-b37a-0ac7b16726b1', N'EdTech Conference 2026',         N'Education & Research',         N'22222222-0000-0000-0000-000000000004', CAST(N'2026-05-27T00:00:00.0000000' AS DateTime2), N'Discover the latest technologies transforming education systems.',                            N'Active')
GO

-- Seed Sessions
INSERT [dbo].[Sessions] ([SessionId], [EventId], [SessionTitle], [SpeakerId], [Description], [SessionStart], [SessionEnd], [SessionUrl]) VALUES
(N'f93aabd6-e10c-41f0-8f3e-1c7c03d4e95f', N'e0c42a82-146c-4b09-8af5-b7b3a1fe2804', N'Introduction to Artificial Intelligence', N'fe77fd80-16c1-4464-9371-c0a0864af060', N'Overview of AI concepts, machine learning basics, and real-world applications.', CAST(N'2026-04-22T10:00:00.0000000' AS DateTime2), CAST(N'2026-04-22T11:30:00.0000000' AS DateTime2), N'https://www.zoom.com/'),
(N'46e15c36-ca8e-4ab8-b863-115c793dfd2e', N'73e35241-1d0d-44db-9d64-7d2bf67e81e0', N'Scaling Your Startup',                   N'b1685fbe-8d78-40ba-8d44-af9e1956eac0', N'Strategies to grow startups and handle business challenges.',                   CAST(N'2026-04-23T10:30:00.0000000' AS DateTime2), CAST(N'2026-04-23T12:00:00.0000000' AS DateTime2), N'https://www.zoom.com/'),
(N'df6c8627-674d-464e-9190-56c5d97f3e4d', N'2295c20c-0163-42fd-864b-ce0e2fcde1bd', N'Mental Wellness & Stress Management',    N'50c12f15-087d-4261-926f-5f56088ecd11', N'Techniques to reduce stress and improve mental health.',                        CAST(N'2026-04-24T08:00:00.0000000' AS DateTime2), CAST(N'2026-04-24T09:30:00.0000000' AS DateTime2), N'https://www.zoom.com/'),
(N'145a8382-7934-4e59-8a1b-9aa527521962', N'978719fa-cd5a-4987-8714-449c3c4fffaa', N'Future of Smart Factories',              N'14844750-01bb-4d74-9cce-76d821705cad', N'Industry 4.0, IoT in manufacturing, and automation insights.',                  CAST(N'2026-04-25T09:30:00.0000000' AS DateTime2), CAST(N'2026-04-25T11:00:00.0000000' AS DateTime2), N'https://www.zoom.com/'),
(N'e316dfb1-45eb-46fd-97f9-fa811230befd', N'a38141c8-fd03-41a3-8658-14c80d6785e1', N'Study Abroad Opportunities',             N'c0015731-3283-4b51-9e13-391f3fda7f13', N'Guidance on studying abroad and admission processes.',                          CAST(N'2026-04-26T11:30:00.0000000' AS DateTime2), CAST(N'2026-04-26T13:00:00.0000000' AS DateTime2), N'https://www.zoom.com/'),
(N'245d869a-979a-43b4-8dfe-d348e2bc065f', N'5a820b41-ebb7-49ec-a0e1-e900f43017e6', N'Understanding Blockchain Fundamentals', N'4b048311-fea3-4638-b960-2f4ac1b9d725', N'Learn blockchain basics, decentralization, and crypto ecosystem.',              CAST(N'2026-05-05T11:00:00.0000000' AS DateTime2), CAST(N'2026-05-05T12:30:00.0000000' AS DateTime2), N'https://www.zoom.com/'),
(N'3137f302-d2e7-481c-8dde-4c551b0ef780', N'82d0d7b2-3e3f-401e-99b1-ae993fb02d5d', N'How to Pitch to Investors',              N'a0d28d9b-6d1e-4742-885b-53d89184557a', N'Learn investor pitching techniques and funding strategies.',                    CAST(N'2026-05-08T13:00:00.0000000' AS DateTime2), CAST(N'2026-05-08T14:30:00.0000000' AS DateTime2), N'https://www.zoom.com/'),
(N'72e573b6-1749-41f4-a0b2-d331941c1c3f', N'714be219-3528-4e59-be50-ce2eee89e7cb', N'Robotics in Industrial Automation',      N'9e1a09f6-ed0c-4b9e-ac3f-c16b1dacd866', N'Role of robotics in increasing industrial efficiency.',                         CAST(N'2026-05-10T14:00:00.0000000' AS DateTime2), CAST(N'2026-05-10T15:30:00.0000000' AS DateTime2), N'https://www.zoom.com/'),
(N'90b1e783-0938-437c-b626-7f7b76db10f7', N'8a5ffc9f-bdc1-448c-9c9d-795e589606c0', N'Guided Meditation for Beginners',        N'e20247f0-6521-4194-abb9-23589ea0ee31', N'Step-by-step meditation techniques for beginners.',                             CAST(N'2026-05-22T07:00:00.0000000' AS DateTime2), CAST(N'2026-05-22T08:30:00.0000000' AS DateTime2), N'https://www.zoom.com/'),
(N'2c568025-5a6d-4dc8-8faa-dc5b139362d0', N'5ba8d7f9-334e-4e8f-b37a-0ac7b16726b1', N'Future of Digital Learning',             N'e6f1047d-03df-4690-8517-c659182f9a77', N'Trends and innovations in online education platforms.',                         CAST(N'2026-05-27T15:00:00.0000000' AS DateTime2), CAST(N'2026-05-27T16:30:00.0000000' AS DateTime2), N'https://www.zoom.com/')
GO

-- Seed ParticipantEvents
INSERT [dbo].[ParticipantEvents] ([Id], [ParticipantEmailId], [EventId], [IsAttended]) VALUES
(N'e054b355-b15c-4d2b-97f0-8037cd3c75b5', N'umeshunde09@gmail.com',            N'e0c42a82-146c-4b09-8af5-b7b3a1fe2804', 0),
(N'71ca342f-4d2c-4e53-bf2a-2b6bfac4bfa1', N'tejasmote.intern@gmail.com',       N'73e35241-1d0d-44db-9d64-7d2bf67e81e0', 0),
(N'3d617137-8335-4485-b06e-24393dfa8d48', N'shritejnagre.coder@gmail.com',     N'2295c20c-0163-42fd-864b-ce0e2fcde1bd', 0),
(N'25b1ca57-f825-40d1-a68e-4c2491f85c1b', N'shivamboradhe@gmail.com',          N'978719fa-cd5a-4987-8714-449c3c4fffaa', 0),
(N'4d1ebf7b-40ac-4b87-ab21-4494418fd557', N'rohitsharma.dev@gmail.com',        N'a38141c8-fd03-41a3-8658-14c80d6785e1', 0),
(N'd99237a9-0e9c-49d9-8b10-f75122770a82', N'priyamehta.tech@gmail.com',        N'5a820b41-ebb7-49ec-a0e1-e900f43017e6', 0),
(N'9f956e0f-82da-40fc-aa1c-a0bb47460453', N'ankitdudhe.codes@gmail.com',       N'82d0d7b2-3e3f-401e-99b1-ae993fb02d5d', 0),
(N'9c2a29a1-99f3-4f3c-996d-5493e94110e7', N'atharvabibave@gmail.com',          N'714be219-3528-4e59-be50-ce2eee89e7cb', 0),
(N'afe92aec-6228-4300-b1d5-cd3ed8940e79', N'ankitdudhe.codes@gmail.com',       N'8a5ffc9f-bdc1-448c-9c9d-795e589606c0', 0),
(N'f661f7b5-7b2a-4603-987f-dbcb68f4cbdb', N'adityashinde.developer@gmail.com', N'5ba8d7f9-334e-4e8f-b37a-0ac7b16726b1', 0),
(N'db0aaa05-b088-479e-ae28-d42f04303119', N'yogeshbarure9@gmail.com',          N'2295c20c-0163-42fd-864b-ce0e2fcde1bd', 0)
GO
