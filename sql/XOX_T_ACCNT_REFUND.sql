--DROP TABLE XOX_T_ACCNT_REFUND

CREATE TABLE XOX_T_ACCNT_REFUND
(
 ROW_ID BIGINT IDENTITY(1,1) NOT NULL
,CREATED_BY BIGINT
,CREATED DATETIME
,LAST_UPD_BY BIGINT
,LAST_UPD DATETIME

,ACCNT_ID BIGINT
,NAME VARCHAR(100)
,MSISDN VARCHAR(20)
,[PLAN] VARCHAR(50)
,TERMINATION_DATE DATETIME
,DEPOSIT DECIMAL(18,2)
,ADVANCE_PAYMENT DECIMAL(18,2)
,BILL_PAYMENT DECIMAL(18,2)
,[USAGE] DECIMAL(18,2)
,TOTAL_REFUND DECIMAL(18,2)
,REFUND_DATE DATETIME
,REMARKS VARCHAR(255)
,[STATUS] INT
 CONSTRAINT XOX_T_ACCNT_REFUND_P01 PRIMARY KEY CLUSTERED 
(
	ROW_ID
)
CONSTRAINT FK_XOX_T_ACCNT_REFUND FOREIGN KEY (ACCNT_ID) REFERENCES XOX_T_ACCNT(ROW_ID)
)