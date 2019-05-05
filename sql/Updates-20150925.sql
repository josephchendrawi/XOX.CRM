alter table XOX_T_ORDER_ACT
drop column returned;

alter table XOX_T_ORDER_ACT
drop column complete;

alter table XOX_T_ORDER_ACT
drop column reason;

alter table XOX_T_ORDER_ACT
drop column install_dt;

alter table XOX_T_ORDER_ACT
add DUE_DATE datetime;

alter table XOX_T_ORDER_ACT
add ACT_REMARKS nvarchar(255);