[v] construct EFmodels
	add / models/EFModels/folder
	construct AppDbContext,Connection String,Entity Classes
	-----------------------------------------------------------------------------
會員系統
[working on]add 註冊新會員
	add/Models/Infra/HashUtility.cs
	add AppSetting,<add key="salt" value="@7417su3a8n3xk"/>

	[workingon]實作註冊功能
		add RegisterVm
		add擴充方法
		add MembersController,Register Action
			add Register.cshtml,RegisterConFirm.cshtml(不必寫action)
		modify _layout.cshtml,add Register link

[]針對新會員暫時沒作發信功能
