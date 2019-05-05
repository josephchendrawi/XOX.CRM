alter table [XOX_T_ORDER]
add [PLAN] nvarchar(255)

alter table [XOX_T_ORDER]
add [REMARKS] nvarchar(MAX)

ALTER TABLE [XOX_T_ADDR]
ALTER COLUMN [STATE] nvarchar(100) NULL