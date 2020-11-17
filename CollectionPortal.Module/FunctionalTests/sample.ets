;#DropDB CollectionPortalEasyTest

#Application CollectionPortalWin
#Application CollectionPortalWeb

*FillForm
 User Name = Admin
*Action Log In

#IfDef CollectionPortalWin

*Action About...
 
*Action OK

#EndIf

#IfDef CollectionPortalWeb
*Action Navigation(My Details)
#EndIf
