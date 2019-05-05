alter table XOX_T_ACCNT
DROP COLUMN REGISTRATION_DT

alter table XOX_T_ORDER
add PORT_REQ_FORM nvarchar(50),
	PORT_ID nvarchar(255)