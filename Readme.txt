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
	信裡的網址，為https://localhost:44300/Members/ActiveRegister?memberId=5&confirmCode=2158987da83449aa95484e98b4f86904
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
	要加[Authorize]	
	modify MemberController,add Index action
	add Views/Members/Index.cshtml(空白範本),填入兩個超連結:"修改個人基本資料","重設密碼"


[V] 實作 修改個人基本資料
	modify MembersController,add EditProfile action , 要加[Authorize]
	add EditProfileVm,EditProfileDto classes
    不允許修改 account,password
    增加Mapping config
	add EditProfile view page

[V]實作 變更密碼
	modify MemberController,add ChangePassword action,要加[Authorize]
	add ChangePasswordVm,ChangePasswordDto classes
		增加Mapping config
	add ChangePassword view page
	modify MemberService,add ChangePassword method
0913進度=====================================================================================================
[V] 實作 忘記密碼/重設密碼
    add / Models/Infra/EmailHelper class
        暫時建立 /files/ folder, 用來存放Email 內容

    modify Login.cshtml, add "忘記密碼" link
    add ForgotPasswordVm, ForgotPasswordDto classes
        增加Mapping config
    add MembersController .ForgotPassword action
        add ForgotPassword.cshtml
        add ForgotPassworConfirm.cshtml

    add ResetPasswordVm, ResetPassworDto classes, 用來輸入密碼
    add MemberController .ResetPassword action
        add ResetPassword.cshtml
        add ResetPasswordConfirm.cshtml

前台購物車
[V] 顯示商品清單
	add ProductIndexVm, ProductIndexDto classes
    增加Mapping config

	 add ProductsController
		add Index action

	add Index.cshtml
    寫js，實作加入購物車功能
    只有登入帳號下才能加入購物車
    將檔案路徑固定為Products/Index
    修改_Layout.cshtml裡，首頁的超連結
	 add CartController，實作將商品加入購物車
		add CartVm, CartItemVm classes
[V]顯示購物車資訊，時作增減數量
- modify CartController, add Info action
    修改_Layout.cshtml裡實作[Cart/Info]的超連結
- add Cart.UpdateItem action
- add UpdateItem.cshtml

[]針對新會員暫時沒作發信功能
