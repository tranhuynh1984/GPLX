
  PRINT '************* START *************'

  PRINT '************* CREATE BACKUP TABLE *************'

  IF OBJECT_ID(N'dbo.FunctionsBackup', N'U') IS NOT NULL  
  begin
    DROP TABLE [dbo].FunctionsBackup;  
  end
  GO
  SELECT * INTO FunctionsBackup FROM Functions 
  declare @rootYearFuncs int
  select @rootYearFuncs = Id from Functions where Name = N'Quản lý tài chính năm'

  select * into #scFuncs from Functions where ParentId = @rootYearFuncs and len(ISNULL(Url,'')) = 0



  declare @scFuncsCount INT
  select @scFuncsCount = COUNT(*) from #scFuncs

  IF @scFuncsCount > 0
		BEGIN
			PRINT '**************************************'
			
			select * into #targetFuncs from Functions where ParentId IN (select Id from #scFuncs)
			
			delete from Functions where Id in (select Id from #targetFuncs)


			
			PRINT '***************************************'

			PRINT '************* UPDATE START*************'
			declare @forID int, @forUnique nvarchar(500), @forFunc nvarchar(500),@fRefId int
			declare cs cursor for 
				select Id, [Unique] from #scFuncs

			open cs

			fetch next from cs
				into @forID,@forUnique

			WHILE @@FETCH_STATUS = 0  
			BEGIN  
			   set @forFunc =
					(select CASE @forUnique 
					WHEN 'CashFollow@View' then '/CashFollow/List?fOps=true'
					WHEN 'RevenuePlan@View' then '/RevenuePlan/List?fOps=true'
					WHEN 'ProfitPlan@View' then '/ProfitPlan/List?fOps=true'
					WHEN 'InvestmentPlan@View' then '/InvestmentPlan/List?fOps=true'
					WHEN 'Dashboard@View'  then '/Dashboard/List?fOps=trye'
					ELSE '' END)

				update Functions set Url = @forFunc where Id = @forID

				select TOP (1) @fRefId = Id from #targetFuncs where CHARINDEX('-8888', Url) > 0 and [Unique] = @forUnique
				
				PRINT '************* Mapping older permission  *************'
				insert into GroupsPermission(GroupId,FunctionId,Permission)
					select GroupId,@forID,Permission from GroupsPermission where FunctionId = @fRefId

				PRINT 'Update Functions ID'
				print @forID
				print 'SET [URL] = ' + @forFunc

			   FETCH NEXT FROM cs into @forID,@forUnique;  
			END 
			CLOSE cs;  
			DEALLOCATE cs; 

			PRINT '************* UPDATE END **************'
			drop table #targetFuncs
		END

  delete from Functions where Url = '/Dashboard/List?fOps=trye'
  drop table #scFuncs

