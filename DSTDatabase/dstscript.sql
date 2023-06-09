IF(NOT EXISTS(SELECT 1 FROM dbo.ApplicationProccess))
BEGIN

SET IDENTITY_INSERT [dbo].[ApplicationProccess] ON 

INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (18, 1, 4, 1, 1, 0, 1, 0, 1, 1, 1, 1, 3, 9, N'START', CAST(N'2020-07-11T21:04:48.907' AS DateTime), 1, CAST(N'2020-07-11T21:21:23.460' AS DateTime), 1, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (19, 1, 3, 2, 1, 0, 1, 0, 1, 1, 1, 1, 5, 4, N'NEXT', CAST(N'2020-07-11T21:06:29.710' AS DateTime), 1, CAST(N'2020-07-11T21:21:53.960' AS DateTime), 1, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (20, 1, 5, 3, 1, 0, 1, 0, 0, 0, 1, 1, 9, 3, N'END', CAST(N'2020-07-11T21:09:59.757' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (21, 1, 4, 4, 1, 0, 1, 0, 0, 0, 1, 1, 3, 9, N'BEGIN', CAST(N'2020-07-11T21:11:25.417' AS DateTime), 1, CAST(N'2021-05-28T08:49:23.723' AS DateTime), 3, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (22, 1, 3, 5, 1, 0, 1, 0, 0, 0, 1, 1, 5, 4, N'NEXT', CAST(N'2020-07-11T21:12:51.037' AS DateTime), 1, CAST(N'2021-05-28T08:49:39.657' AS DateTime), 3, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (23, 1, 5, 6, 1, 0, 1, 0, 0, 0, 1, 1, 9, 3, N'DONE', CAST(N'2020-07-11T21:14:30.163' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (24, 5, 4, 1, 1, 0, 1, 0, 1, 1, 1, 1, 3, 9, N'START', CAST(N'2021-05-14T17:06:20.460' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (25, 5, 3, 2, 1, 0, 1, 0, 1, 1, 1, 1, 5, 4, N'NEXT', CAST(N'2021-05-14T17:07:33.493' AS DateTime), 1, CAST(N'2021-05-23T08:48:59.017' AS DateTime), 3, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (26, 5, 5, 3, 1, 0, 1, 0, 0, 0, 1, 1, 9, 3, N'END', CAST(N'2021-05-14T17:08:42.757' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (27, 6, 4, 1, 1, 0, 1, 0, 1, 1, 1, 1, 3, 9, N'START', CAST(N'2021-05-14T17:09:33.623' AS DateTime), 1, CAST(N'2021-05-23T08:49:18.887' AS DateTime), 3, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (28, 6, 3, 2, 1, 0, 1, 0, 1, 1, 1, 1, 5, 4, N'NEXT', CAST(N'2021-05-14T17:11:06.870' AS DateTime), 1, CAST(N'2021-05-23T08:49:32.783' AS DateTime), 3, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (29, 6, 5, 3, 1, 0, 1, 0, 0, 1, 1, 1, 9, 3, N'END', CAST(N'2021-05-14T17:12:06.530' AS DateTime), 1, CAST(N'2021-05-23T08:49:47.140' AS DateTime), 3, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (30, 9, 4, 1, 1, 0, 1, 0, 1, 1, 1, 1, 3, 9, N'START', CAST(N'2021-05-14T17:14:58.523' AS DateTime), 1, CAST(N'2021-05-23T08:50:03.067' AS DateTime), 3, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (31, 9, 3, 2, 1, 0, 1, 0, 1, 1, 1, 1, 5, 4, N'NEXT', CAST(N'2021-05-14T17:15:36.663' AS DateTime), 1, CAST(N'2021-05-23T08:50:16.550' AS DateTime), 3, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (32, 9, 5, 3, 1, 0, 1, 0, 0, 0, 1, 1, 6, 3, N'NEXT', CAST(N'2021-05-14T17:16:29.057' AS DateTime), 1, CAST(N'2021-07-28T11:22:57.053' AS DateTime), 4, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (33, 9, 6, 4, 1, 0, 1, 0, 0, 0, 1, 1, 9, 5, N'END', CAST(N'2021-05-14T17:17:17.860' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (34, 7, 4, 1, 1, 0, 1, 0, 1, 1, 1, 1, 3, 9, N'START', CAST(N'2021-05-14T17:19:21.387' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (35, 7, 3, 2, 1, 0, 1, 0, 1, 1, 1, 1, 5, 4, N'NEXT', CAST(N'2021-05-14T17:21:03.893' AS DateTime), 1, CAST(N'2021-05-23T08:50:35.580' AS DateTime), 3, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (36, 7, 5, 3, 1, 0, 1, 0, 1, 0, 1, 1, 6, 3, N'NEXT', CAST(N'2021-05-14T17:22:02.010' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (37, 7, 6, 4, 1, 0, 1, 0, 0, 0, 1, 1, 9, 5, N'END', CAST(N'2021-05-14T17:23:14.420' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (38, 8, 4, 1, 1, 0, 1, 0, 1, 1, 1, 1, 3, 9, N'START', CAST(N'2021-05-14T17:25:16.100' AS DateTime), 1, CAST(N'2021-05-23T08:50:58.373' AS DateTime), 3, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (39, 8, 3, 2, 1, 0, 1, 0, 1, 1, 1, 1, 5, 4, N'NEXT', CAST(N'2021-05-14T17:26:36.727' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (40, 8, 5, 3, 1, 0, 1, 0, 0, 0, 1, 1, 6, 3, N'NEXT', CAST(N'2021-05-14T17:27:19.607' AS DateTime), 1, CAST(N'2021-05-23T08:50:45.973' AS DateTime), 3, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (41, 8, 6, 4, 1, 0, 1, 0, 0, 0, 1, 1, 9, 5, N'END', CAST(N'2021-05-14T17:28:07.930' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (42, 7, 4, 5, 1, 0, 1, 0, 1, 1, 1, 1, 3, 9, N'BEGIN', CAST(N'2021-05-14T17:34:49.223' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (43, 7, 3, 6, 1, 0, 1, 0, 1, 0, 1, 1, 5, 4, N'NEXT', CAST(N'2021-05-14T17:35:33.073' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (44, 7, 5, 7, 1, 0, 1, 0, 0, 0, 1, 1, 9, 3, N'DONE', CAST(N'2021-05-14T17:36:25.570' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (45, 8, 4, 5, 1, 0, 1, 0, 1, 1, 1, 1, 3, 9, N'BEGIN', CAST(N'2021-05-14T17:37:45.820' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (46, 8, 3, 6, 1, 0, 1, 0, 1, 1, 1, 1, 5, 4, N'NEXT', CAST(N'2021-05-14T17:38:24.020' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (47, 8, 5, 7, 1, 0, 1, 0, 0, 0, 1, 1, 9, 3, N'DONE', CAST(N'2021-05-14T17:39:21.303' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (48, 10, 4, 1, 1, 0, 1, 0, 1, 1, 1, 1, 3, 9, N'START', CAST(N'2021-07-28T10:20:53.773' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (49, 10, 3, 2, 1, 0, 1, 0, 1, 1, 1, 1, 5, 4, N'NEXT', CAST(N'2021-07-28T10:21:40.837' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (50, 10, 5, 3, 1, 0, 1, 0, 0, 1, 1, 1, 6, 3, N'NEXT', CAST(N'2021-07-28T10:22:39.663' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationProccess] ([ProccessID], [StageID], [RoleID], [Sort], [LocationID], [canPush], [canWork], [canInspect], [canSchdule], [canReport], [canAccept], [canReject], [onAcceptRoleID], [onRejectRoleID], [Process], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (51, 10, 6, 4, 1, 0, 1, 0, 0, 0, 1, 1, 9, 5, N'END', CAST(N'2021-07-28T10:23:22.133' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[ApplicationProccess] OFF
END;

GO

IF(NOT EXISTS(SELECT 1 FROM dbo.ApplicationStage))
BEGIN

SET IDENTITY_INSERT [dbo].[ApplicationStage] ON 

INSERT [dbo].[ApplicationStage] ([AppStageID], [StageName], [ShortName], [Amount], [ServiceCharge], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1, N'DRILL STEM TEST', N'DST', 100000, 0, CAST(N'2020-07-10T06:52:19.530' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationStage] ([AppStageID], [StageName], [ShortName], [Amount], [ServiceCharge], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (5, N'OFF CYCLE MER', N'OC-MER', 250000, 0, CAST(N'2021-05-11T11:59:48.287' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationStage] ([AppStageID], [StageName], [ShortName], [Amount], [ServiceCharge], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (6, N'ROUTINE MER', N'R-MER', 250000, 0, CAST(N'2021-05-11T12:00:22.497' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationStage] ([AppStageID], [StageName], [ShortName], [Amount], [ServiceCharge], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (7, N'NEW EWT', N'N-EWT', 5000, 0, CAST(N'2021-05-11T12:02:52.670' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationStage] ([AppStageID], [StageName], [ShortName], [Amount], [ServiceCharge], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (8, N'EXTENSION FOR EWT', N'E-EWT', 10000, 0, CAST(N'2021-05-11T12:03:23.983' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ApplicationStage] ([AppStageID], [StageName], [ShortName], [Amount], [ServiceCharge], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (9, N'ROUTINE TECHNICAL ALLOWABLE RATE', N'RTAR', 250000, 0, CAST(N'2021-05-14T17:13:08.850' AS DateTime), CAST(N'2021-07-28T09:51:53.253' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[ApplicationStage] ([AppStageID], [StageName], [ShortName], [Amount], [ServiceCharge], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (10, N'OFF CYCLE TECHNICAL ALLOWABLE RATE', N'OTAR', 250000, 0, CAST(N'2021-07-28T09:52:30.803' AS DateTime), NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[ApplicationStage] OFF
END;

GO


IF(NOT EXISTS(SELECT 1 FROM dbo.ApplicationType))
BEGIN
SET IDENTITY_INSERT [dbo].[ApplicationType] ON 

INSERT [dbo].[ApplicationType] ([AppTypeID], [TypeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1, N'DRILL STEM TEST (DST)', CAST(N'2021-05-11T11:26:37.187' AS DateTime), CAST(N'2021-05-11T11:37:10.730' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[ApplicationType] ([AppTypeID], [TypeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (2, N'EXTENDED WELL TEST (EWT)', CAST(N'2021-05-11T11:29:21.273' AS DateTime), CAST(N'2021-05-11T11:36:55.790' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[ApplicationType] ([AppTypeID], [TypeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (3, N'MAXIMUM EFFICIENT RATE (MER)', CAST(N'2021-05-11T11:36:44.680' AS DateTime), CAST(N'2021-05-11T12:04:44.340' AS DateTime), 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[ApplicationType] OFF
END;

GO

IF(NOT EXISTS(SELECT 1 FROM dbo.AppTypeStage))
BEGIN
SET IDENTITY_INSERT [dbo].[AppTypeStage] ON 

INSERT [dbo].[AppTypeStage] ([TypeStageID], [AppTypeID], [AppStageID], [Counter], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1, 1, 1, 1, CAST(N'2021-05-11T12:20:47.400' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[AppTypeStage] ([TypeStageID], [AppTypeID], [AppStageID], [Counter], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (2, 2, 7, 1, CAST(N'2021-05-11T12:21:15.180' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[AppTypeStage] ([TypeStageID], [AppTypeID], [AppStageID], [Counter], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (3, 2, 8, 2, CAST(N'2021-05-11T12:21:27.530' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[AppTypeStage] ([TypeStageID], [AppTypeID], [AppStageID], [Counter], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (4, 3, 5, 1, CAST(N'2021-05-11T12:21:41.147' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[AppTypeStage] ([TypeStageID], [AppTypeID], [AppStageID], [Counter], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (5, 3, 6, 2, CAST(N'2021-05-11T12:22:00.513' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[AppTypeStage] ([TypeStageID], [AppTypeID], [AppStageID], [Counter], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (6, 3, 9, 3, CAST(N'2021-05-14T17:13:37.410' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[AppTypeStage] ([TypeStageID], [AppTypeID], [AppStageID], [Counter], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (7, 3, 10, 4, CAST(N'2021-07-28T09:53:12.067' AS DateTime), NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[AppTypeStage] OFF
END;


GO

IF(NOT EXISTS(SELECT 1 FROM dbo.FieldOffices))
BEGIN
SET IDENTITY_INSERT [dbo].[FieldOffices] ON 

INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1, N'IBADAN FIELD OFFICE', CAST(N'2018-08-11T23:09:32.673' AS DateTime), CAST(N'2018-08-11T23:44:06.170' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (2, N'ILORIN FIELD OFFICE', CAST(N'2018-08-11T23:46:48.523' AS DateTime), CAST(N'2018-08-11T23:50:56.580' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (3, N'ABEOKUTA FIELD OFFICE', CAST(N'2018-08-11T23:47:09.877' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (4, N'EKET FIELD OFFICE', CAST(N'2018-08-11T23:47:26.917' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (5, N'YENOGOA FIELD OFFICE', CAST(N'2018-08-11T23:47:58.920' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (6, N'MAKURDI FIELD OFFICE', CAST(N'2018-08-11T23:48:16.517' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (7, N'LOKOJA FIELD OFFICE', CAST(N'2018-08-11T23:48:26.797' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (8, N'MINNA FIELD OFFICE', CAST(N'2018-08-11T23:48:46.347' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (9, N'JOS FIELD OFFICE', CAST(N'2018-08-11T23:49:06.243' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (10, N'AKURE FIELD OFFICE', CAST(N'2018-08-11T23:50:09.247' AS DateTime), CAST(N'2018-08-16T13:13:22.227' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (11, N'KANO FIELD OFFICE', CAST(N'2018-08-11T23:50:21.533' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (12, N'SOKOTO FIELD OFFICE', CAST(N'2018-08-11T23:51:22.920' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (13, N'YOLA FIELD OFFICE', CAST(N'2018-08-11T23:52:01.890' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (14, N'GOMBE FIELD OFFICE', CAST(N'2018-08-11T23:52:39.400' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (15, N'ENUGU FIELD OFFICE', CAST(N'2018-08-11T23:52:51.217' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (16, N'KASTINA FIELD OFFICE', CAST(N'2018-08-11T23:53:10.590' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (17, N'BENIN FIELD OFFICE', CAST(N'2018-08-11T23:53:29.980' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (18, N'CALABAR FIELD OFFICE', CAST(N'2018-08-11T23:53:47.053' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (19, N'BAUCHI FIELD OFFICE', CAST(N'2018-08-11T23:54:04.303' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (20, N'GUSAU FIELD OFFICE', CAST(N'2018-08-11T23:54:15.307' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (21, N'UMUAHIA FIELD OFFICE', CAST(N'2018-08-11T23:54:50.257' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (22, N'ABA FIELD OFFICE', CAST(N'2018-08-12T00:09:16.793' AS DateTime), CAST(N'2019-03-29T07:46:47.443' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1002, N'LAGOS FIELD OFFICE', CAST(N'2018-08-31T13:15:55.673' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1003, N'LAGOS HEAD OFFICE', CAST(N'2018-08-31T13:16:24.917' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1004, N'ABUJA FIELD OFFICE', CAST(N'2018-08-31T13:16:46.177' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1005, N'KADUNA FIELD OFFICE', CAST(N'2018-08-31T13:16:59.797' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1006, N'MAIDUGURI FIELD OFFICE', CAST(N'2018-08-31T13:17:14.787' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1007, N'OWERRI FIELD OFFICE', CAST(N'2018-08-31T13:17:28.243' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1008, N'WARRI FIELD OFFICE', CAST(N'2018-08-31T13:17:56.813' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[FieldOffices] ([FieldOffice_id], [OfficeName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1009, N'PORTHARCOURT FIELD OFFICE', CAST(N'2018-08-31T13:18:10.770' AS DateTime), NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[FieldOffices] OFF
END;


GO


IF(NOT EXISTS(SELECT 1 FROM dbo.Location))
BEGIN
SET IDENTITY_INSERT [dbo].[Location] ON 

INSERT [dbo].[Location] ([LocationID], [LocationName], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedAt], [DeletedBy]) VALUES (1, N'HQ', CAST(N'2019-11-22T09:57:59.023' AS DateTime), 1, CAST(N'2019-11-22T14:54:22.173' AS DateTime), 1, 0, NULL, NULL)
INSERT [dbo].[Location] ([LocationID], [LocationName], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedAt], [DeletedBy]) VALUES (2, N'FO', CAST(N'2019-11-22T14:54:35.227' AS DateTime), 1, CAST(N'2020-07-10T06:50:22.700' AS DateTime), 1, 0, NULL, NULL)
INSERT [dbo].[Location] ([LocationID], [LocationName], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedAt], [DeletedBy]) VALUES (3, N'ST', CAST(N'2019-11-22T14:55:18.927' AS DateTime), 1, CAST(N'2019-11-22T14:55:22.837' AS DateTime), 1, 1, CAST(N'2019-11-22T14:55:22.837' AS DateTime), 1)
INSERT [dbo].[Location] ([LocationID], [LocationName], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [DeleteStatus], [DeletedAt], [DeletedBy]) VALUES (4, N'CUS', CAST(N'2019-11-24T08:22:36.890' AS DateTime), 1, NULL, NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Location] OFF
END;

GO


IF(NOT EXISTS(SELECT 1 FROM dbo.UserRoles))
BEGIN
SET IDENTITY_INSERT [dbo].[UserRoles] ON 

INSERT [dbo].[UserRoles] ([Role_id], [RoleName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1, N'SUPER ADMIN', CAST(N'2019-11-22T14:12:14.937' AS DateTime), CAST(N'2019-11-22T14:15:00.793' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[UserRoles] ([Role_id], [RoleName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (2, N'ICT ADMIN', CAST(N'2019-11-22T14:14:09.197' AS DateTime), CAST(N'2020-04-14T06:44:34.947' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[UserRoles] ([Role_id], [RoleName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (3, N'MANAGER RS', CAST(N'2019-11-25T15:01:52.307' AS DateTime), CAST(N'2020-04-24T21:44:28.260' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[UserRoles] ([Role_id], [RoleName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (4, N'TEAM', CAST(N'2019-11-26T07:08:06.763' AS DateTime), CAST(N'2020-04-14T07:04:18.340' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[UserRoles] ([Role_id], [RoleName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (5, N'AD RM', CAST(N'2019-11-26T07:10:52.420' AS DateTime), CAST(N'2020-04-24T21:44:10.693' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[UserRoles] ([Role_id], [RoleName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (6, N'HEAD UMR', CAST(N'2019-11-26T07:11:56.973' AS DateTime), CAST(N'2020-04-14T06:45:02.290' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[UserRoles] ([Role_id], [RoleName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (7, N'HOOD', CAST(N'2019-11-26T09:46:44.077' AS DateTime), CAST(N'2020-04-14T06:46:09.267' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[UserRoles] ([Role_id], [RoleName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (8, N'ADMIN', CAST(N'2019-11-26T09:47:23.627' AS DateTime), CAST(N'2020-04-14T06:45:36.113' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[UserRoles] ([Role_id], [RoleName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (9, N'COMPANY', CAST(N'2019-12-02T10:34:01.133' AS DateTime), CAST(N'2020-07-10T06:50:36.577' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[UserRoles] ([Role_id], [RoleName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (10, N'DIRECTOR', CAST(N'2020-03-25T07:56:27.573' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[UserRoles] ([Role_id], [RoleName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (11, N'ZOPSCON', CAST(N'2020-08-04T05:57:10.487' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[UserRoles] ([Role_id], [RoleName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (12, N'UMR ZONE HEAD', CAST(N'2020-08-04T05:57:25.860' AS DateTime), NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[UserRoles] OFF
END;


GO


IF(NOT EXISTS(SELECT 1 FROM dbo.ZonalOffice))
BEGIN
SET IDENTITY_INSERT [dbo].[ZonalOffice] ON 

INSERT [dbo].[ZonalOffice] ([Zone_id], [ZoneName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1, N'LAGOS ZONAL OFFICE', CAST(N'2018-08-13T13:04:14.157' AS DateTime), CAST(N'2020-07-10T06:55:13.133' AS DateTime), 0, 1, CAST(N'2018-08-15T19:33:34.813' AS DateTime))
INSERT [dbo].[ZonalOffice] ([Zone_id], [ZoneName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (2, N'ABUJA ZONAL OFFICE', CAST(N'2018-08-13T13:35:10.163' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZonalOffice] ([Zone_id], [ZoneName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (3, N'KADUNA ZONAL OFFICE', CAST(N'2018-08-13T13:35:19.850' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZonalOffice] ([Zone_id], [ZoneName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (4, N'MAIDUGURI ZONAL OFFICE', CAST(N'2018-08-13T13:35:31.830' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZonalOffice] ([Zone_id], [ZoneName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (5, N'OWERRI ZONAL OFFICE', CAST(N'2018-08-13T13:35:53.010' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZonalOffice] ([Zone_id], [ZoneName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (6, N'WARRI ZONAL OFFICE', CAST(N'2018-08-13T13:36:02.713' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZonalOffice] ([Zone_id], [ZoneName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (7, N'PH ZONAL OFFICE', CAST(N'2018-08-13T13:36:12.457' AS DateTime), CAST(N'2018-08-14T12:08:11.537' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[ZonalOffice] ([Zone_id], [ZoneName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (8, N'TESTING ZONAL OFFICE', CAST(N'2018-08-13T13:36:45.600' AS DateTime), CAST(N'2019-03-29T10:27:44.620' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[ZonalOffice] ([Zone_id], [ZoneName], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1002, N'LAGOS HEAD OFFICE', CAST(N'2018-08-25T09:26:54.200' AS DateTime), NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[ZonalOffice] OFF
END;



GO


IF(NOT EXISTS(SELECT 1 FROM dbo.ZoneFieldOffice))
BEGIN
SET IDENTITY_INSERT [dbo].[ZoneFieldOffice] ON 

INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1, 1, 1, CAST(N'2018-08-15T19:09:43.907' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (2, 1, 2, CAST(N'2018-08-15T19:10:17.877' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (3, 1, 3, CAST(N'2018-08-16T10:26:50.343' AS DateTime), CAST(N'2019-03-29T12:13:01.517' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (4, 7, 4, CAST(N'2018-08-16T12:16:12.703' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (5, 7, 5, CAST(N'2018-08-16T12:16:26.453' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (6, 2, 6, CAST(N'2018-08-16T12:16:49.813' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (7, 2, 7, CAST(N'2018-08-16T12:17:02.320' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (8, 2, 8, CAST(N'2018-08-16T12:17:12.990' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (9, 2, 9, CAST(N'2018-08-16T12:17:25.323' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (10, 6, 17, CAST(N'2018-08-16T12:17:55.023' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (11, 6, 10, CAST(N'2018-08-16T12:18:12.920' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (12, 3, 11, CAST(N'2018-08-16T12:18:49.303' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (13, 3, 16, CAST(N'2018-08-16T12:18:58.433' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (14, 3, 12, CAST(N'2018-08-16T12:19:08.840' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (15, 3, 20, CAST(N'2018-08-16T12:19:28.107' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (16, 3, 19, CAST(N'2018-08-16T12:19:42.330' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (17, 4, 13, CAST(N'2018-08-16T12:20:14.033' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (18, 4, 14, CAST(N'2018-08-16T12:20:24.453' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (19, 5, 15, CAST(N'2018-08-16T12:20:36.543' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (20, 5, 21, CAST(N'2018-08-16T12:20:49.500' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (21, 7, 18, CAST(N'2018-08-16T12:21:30.087' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (22, 1, 1002, CAST(N'2018-08-31T13:19:40.260' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (23, 1002, 1003, CAST(N'2018-08-31T13:20:01.637' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (24, 2, 1004, CAST(N'2018-08-31T13:20:19.427' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (25, 3, 1005, CAST(N'2018-08-31T13:20:33.153' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (26, 4, 1006, CAST(N'2018-08-31T13:20:46.713' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (27, 5, 1007, CAST(N'2018-08-31T13:21:11.030' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (28, 6, 1008, CAST(N'2018-08-31T13:21:23.027' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneFieldOffice] ([ZoneFieldOffice_id], [Zone_id], [FieldOffice_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (29, 7, 1009, CAST(N'2018-08-31T13:21:34.323' AS DateTime), NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[ZoneFieldOffice] OFF
END;


GO


IF(NOT EXISTS(SELECT 1 FROM dbo.ZoneStates))
BEGIN
SET IDENTITY_INSERT [dbo].[ZoneStates] ON 

INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (1, 1, 1, CAST(N'2018-08-14T09:28:13.037' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (2, 1, 30, CAST(N'2018-08-14T09:28:50.240' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (3, 1, 32, CAST(N'2018-08-14T11:02:17.553' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (4, 1, 33, CAST(N'2018-08-14T11:02:37.847' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (5, 1, 19, CAST(N'2018-08-14T11:02:55.923' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (6, 7, 6, CAST(N'2018-08-14T11:03:34.003' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (7, 7, 18, CAST(N'2018-08-14T11:05:43.673' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (8, 7, 9, CAST(N'2018-08-14T11:06:13.170' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (9, 7, 23, CAST(N'2018-08-14T11:06:36.013' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (10, 2, 2, CAST(N'2018-08-14T11:07:03.460' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (11, 2, 24, CAST(N'2018-08-14T11:07:23.797' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (12, 2, 7, CAST(N'2018-08-14T11:07:57.887' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (13, 2, 29, CAST(N'2018-08-14T11:08:19.700' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (14, 2, 34, CAST(N'2018-08-14T11:08:45.377' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (15, 6, 11, CAST(N'2018-08-14T11:09:00.970' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (16, 6, 26, CAST(N'2018-08-14T11:09:12.017' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (17, 6, 31, CAST(N'2018-08-14T11:09:35.287' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (18, 6, 27, CAST(N'2018-08-14T11:10:04.490' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (19, 3, 10, CAST(N'2018-08-14T11:10:31.413' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (20, 3, 38, CAST(N'2018-08-14T11:10:53.030' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (21, 3, 3, CAST(N'2018-08-14T11:11:09.653' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (22, 3, 20, CAST(N'2018-08-14T11:11:24.587' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (23, 3, 37, CAST(N'2018-08-14T11:11:35.690' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (24, 3, 28, CAST(N'2018-08-14T11:11:48.767' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (25, 3, 8, CAST(N'2018-08-14T11:12:01.797' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (26, 3, 16, CAST(N'2018-08-14T11:12:21.433' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (27, 4, 5, CAST(N'2018-08-14T11:12:47.403' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (28, 4, 21, CAST(N'2018-08-14T11:12:58.547' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (29, 4, 15, CAST(N'2018-08-14T11:13:11.800' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (30, 4, 36, CAST(N'2018-08-14T11:13:26.743' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (31, 4, 35, CAST(N'2018-08-14T11:13:39.867' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (32, 5, 17, CAST(N'2018-08-14T11:13:53.063' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (33, 5, 13, CAST(N'2018-08-14T11:14:09.813' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (34, 5, 12, CAST(N'2018-08-14T11:14:25.097' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (35, 5, 4, CAST(N'2018-08-14T11:14:48.513' AS DateTime), CAST(N'2018-08-14T12:34:15.857' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (36, 5, 25, CAST(N'2018-08-14T11:15:09.863' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (37, 2, 14, CAST(N'2018-08-14T11:25:00.580' AS DateTime), NULL, 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (38, 1, 4, CAST(N'2018-08-14T12:53:33.740' AS DateTime), CAST(N'2019-03-29T12:12:09.357' AS DateTime), 0, NULL, NULL)
INSERT [dbo].[ZoneStates] ([ZoneStates_id], [Zone_id], [State_id], [CreatedAt], [UpdatedAt], [DeleteStatus], [DeletedBy], [DeletedAt]) VALUES (39, 1002, 1, CAST(N'2018-08-31T13:28:32.100' AS DateTime), NULL, 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[ZoneStates] OFF
END;


GO