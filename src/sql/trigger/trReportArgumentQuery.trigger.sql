DROP TRIGGER IF EXISTS dbo.trReportArgumentQuery
GO

---
-- Trigger for ReportArgumentQuery that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trReportArgumentQuery
	ON [dbo].[ReportArgumentQuery]
	FOR UPDATE, DELETE
	AS
		BEGIN
	SET NOCOUNT ON

	DECLARE 
		@AuditLogTransactionId	bigint,
		@Inserted	    		int = 0,
 		@ROWS_COUNT				int

	SELECT @ROWS_COUNT=count(*) from inserted

    -- Check if this is an INSERT, UPDATE or DELETE Action.
    DECLARE @action as varchar(10);
    SET @action = 'INSERT';
    IF EXISTS(SELECT 1 FROM DELETED)
    BEGIN
        SET @action = 
            CASE
                WHEN EXISTS(SELECT 1 FROM INSERTED) THEN 'UPDATE'
                ELSE 'DELETE'
            END
        SELECT @ROWS_COUNT=count(*) from deleted
    END

	-- determine username
	DECLARE @UserName nvarchar(200);
	set @username = current_user

	-- insert parent transaction
	INSERT INTO audit.AuditLogTransaction (TableName, TableSchema, Action, HostName, ApplicationName, AuditLogin, AuditDate, AffectedRows, DatabaseName, UserId, TransactionId)
	values('ReportArgumentQuery', 'dbo', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

	-- [ReportArgumentQueryId]
	IF UPDATE([ReportArgumentQueryId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ReportArgumentQueryId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ReportArgumentQueryId], NEW.[ReportArgumentQueryId]), 0), '[[ReportArgumentQueryId]] Is Null')),
				'[ReportArgumentQueryId]',
				CONVERT(nvarchar(4000), OLD.[ReportArgumentQueryId], 126),
				CONVERT(nvarchar(4000), NEW.[ReportArgumentQueryId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ReportArgumentQueryId], NEW.[ReportArgumentQueryId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ReportArgumentQueryId] = OLD.[ReportArgumentQueryId] or (NEW.[ReportArgumentQueryId] Is Null and OLD.[ReportArgumentQueryId] Is Null))
			WHERE ((NEW.[ReportArgumentQueryId] <> OLD.[ReportArgumentQueryId]) 
					Or (NEW.[ReportArgumentQueryId] Is Null And OLD.[ReportArgumentQueryId] Is Not Null)
					Or (NEW.[ReportArgumentQueryId] Is Not Null And OLD.[ReportArgumentQueryId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ArgQuery]
	IF UPDATE([ArgQuery]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ReportArgumentQueryId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ReportArgumentQueryId], NEW.[ReportArgumentQueryId]), 0), '[[ReportArgumentQueryId]] Is Null')),
				'[ArgQuery]',
				CONVERT(nvarchar(4000), OLD.[ArgQuery], 126),
				CONVERT(nvarchar(4000), NEW.[ArgQuery], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ReportArgumentQueryId], NEW.[ReportArgumentQueryId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ReportArgumentQueryId] = OLD.[ReportArgumentQueryId] or (NEW.[ReportArgumentQueryId] Is Null and OLD.[ReportArgumentQueryId] Is Null))
			WHERE ((NEW.[ArgQuery] <> OLD.[ArgQuery]) 
					Or (NEW.[ArgQuery] Is Null And OLD.[ArgQuery] Is Not Null)
					Or (NEW.[ArgQuery] Is Not Null And OLD.[ArgQuery] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
