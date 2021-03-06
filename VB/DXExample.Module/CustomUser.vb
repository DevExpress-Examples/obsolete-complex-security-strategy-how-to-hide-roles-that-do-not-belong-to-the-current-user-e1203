Imports Microsoft.VisualBasic
Imports System

Imports DevExpress.Xpo

Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Persistent.Validation
Imports DevExpress.Persistent.Base.Security
Imports System.Collections.Generic
Imports System.Security

Namespace DXExample.Module
	<ImageName("BO_User")> _
	Public Class CustomUser
		Inherits Person
		Implements IUserWithRoles, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser
		Private user As UserImpl
		Private permissions_Renamed As List(Of IPermission)
		Public Sub New(ByVal session As Session)
			MyBase.New(session)
			permissions_Renamed = New List(Of IPermission)()
			user = New UserImpl(Me)
		End Sub
		Public Sub ReloadPermissions() Implements DevExpress.Persistent.Base.Security.IUser.ReloadPermissions
			Roles.Reload()
			For Each role As CustomRole In Roles
				role.PersistentPermissions.Reload()
			Next role
		End Sub
		Public Function ComparePassword(ByVal password As String) As Boolean Implements IAuthenticationStandardUser.ComparePassword
			Return user.ComparePassword(password)
		End Function
		Public Sub SetPassword(ByVal password As String) Implements IAuthenticationStandardUser.SetPassword
			user.SetPassword(password)
		End Sub
#If MediumTrust Then
		<Browsable(False), EditorBrowsable(EditorBrowsableState.Never), Persistent> _
		Public Property StoredPassword() As String
			Get
				Return user.StoredPassword
			End Get
			Set(ByVal value As String)
				user.StoredPassword = value
				OnChanged("StoredPassword")
			End Set
		End Property
#Else
		<Persistent> _
		Private Property StoredPassword() As String
			Get
				Return user.StoredPassword
			End Get
			Set(ByVal value As String)
				user.StoredPassword = value
				OnChanged("StoredPassword")
			End Set
		End Property
#End If
		<Association("User-Role")> _
		Public ReadOnly Property Roles() As XPCollection(Of CustomRole)
			Get
				Return GetCollection(Of CustomRole)("Roles")
			End Get
		End Property
		Private ReadOnly Property IUserWithRoles_Roles() As IList(Of IRole) Implements IUserWithRoles.Roles
			Get
				Return New ListConverter(Of IRole, CustomRole)(Roles)
			End Get
		End Property
        Public ReadOnly Property HiddenUserName() As String Implements IUser.UserName, IAuthenticationStandardUser.UserName
            Get
                Return Me.UserName
            End Get
        End Property
        <RuleRequiredField("Fill User Name", "Save", "The user name must not be empty"), RuleUniqueValue("Unique User Name", "Save", "The login with the entered UserName was already registered within the system")> _
        Public Property UserName() As String Implements IAuthenticationActiveDirectoryUser.UserName
            Get
                Return user.UserName
            End Get
            Set(ByVal value As String)
                user.UserName = value
                OnChanged("UserName")
            End Set
        End Property
		Public Property ChangePasswordOnFirstLogon() As Boolean Implements IAuthenticationStandardUser.ChangePasswordOnFirstLogon
			Get
				Return user.ChangePasswordAfterLogon
			End Get
			Set(ByVal value As Boolean)
				user.ChangePasswordAfterLogon = value
				OnChanged("ChangePasswordOnFirstLogon")
			End Set
		End Property
		Public Property IsActive() As Boolean Implements DevExpress.Persistent.Base.Security.IUser.IsActive
			Get
				Return user.IsActive
			End Get
			Set(ByVal value As Boolean)
				user.IsActive = value
				OnChanged("IsActive")
			End Set
		End Property
        Public ReadOnly Property Permissions() As IList(Of IPermission) Implements IUserWithRoles.Permissions
            Get
                permissions_Renamed.Clear()
                For Each role As CustomRole In Roles
                    permissions_Renamed.AddRange(role.Permissions)
                Next role
                Return permissions_Renamed.AsReadOnly()
            End Get
        End Property
	End Class

End Namespace
