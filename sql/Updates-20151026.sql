alter table [XOX_T_ORDER]
add REJECT_REASONS nvarchar(MAX)

ALTER TABLE [XOX_T_ORDER_ACT]
ALTER COLUMN [ACT_DESC] nvarchar(MAX) NULL
