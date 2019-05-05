
--XOX_T_IS_KEY
DROP TABLE XOX_T_IS_KEY

CREATE TABLE XOX_T_IS_KEY
(
 ROW_ID BIGINT IDENTITY(1,1) NOT NULL
,API_KEY NVARCHAR(255)
,LAST_UPD DATETIME
,XOX_IS_ID BIGINT
 CONSTRAINT XOX_T_IS_KEY_P01 PRIMARY KEY CLUSTERED 
(
	ROW_ID
)
)

CREATE UNIQUE INDEX XOX_T_IS_KEY_U01 ON XOX_T_IS_KEY (XOX_IS_ID)