[v] construct EFmodels
	add / models/EFModels/folder
	construct AppDbContext,Connection String,Entity Classes
	---------------------------------------------------------------------------------
會員系統
[V]add 註冊新會員
	add/Models/Infra/HashUtility.cs
	add AppSetting,<add key="salt" value="@7417su3a8n3xk"/>

	[V]實作註冊功能
		add RegisterVm
		add擴充方法
		add MembersController,Register Action
			add Register.cshtml,RegisterConFirm.cshtml(不必寫action)
		modify _layout.cshtml,add Register link
[V]實作 新會員 Email 確認功能
	信裡的網址，為https://localhost:44300/Members/ActiveRegister?memberId=3&confirmCode=0dbd7446c40849218e71b03d51694f4d
	modify MembersController, add ActiveRegister Action
    update isConfirmed=1, confirmCode=null
	add ActiveRegister.cshtml
//--------------------------------第一天-----------------------------------------------
[V] 實作登入登出功能
	只有帳密正確且開通會員才允許登入

	modify web.config, add <authentication mode="Forms">
	add LoginVm, LoginDto

	**安裝automapper package
		add Models/MappingProfile.cs
		modify Global.asax.cs,add Mapper config
		
	modify MembersController, add Login, Logout Actions
	add Login.cshtml
	modify _Layout.cshtml, add Login, Logout links
	modify 將about改稱需要登入才能檢視

	modify MemberService,IMemberRepository,新增Login相關成員
[V]做Members/Index 會員中心頁,登入成功之後，導向此頁
	modify MemberController,add Index action
	add Views/Members/Index.cshtml(空白範本),填入兩個超連結:"修改個人基本資料","重設密碼"
[working on ] 實作 修改個人基本資料
[]要做Member/Index 會員中心頁，登入之後導向此頁
[]針對新會員暫時沒作發信功能
