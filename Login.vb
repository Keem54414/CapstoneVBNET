Imports System.Data.OleDb

Public Class Login
    Public Shared LoggedInUserID As String
    Public Shared LoggedInFirstName As String
    Public Shared LoggedInLastName As String
    Public Shared LoggedInUsername As String
    Public Shared LoggedInUserRole As String

    Private connString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=DatabaseFile"

    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TxtPass.PasswordChar = "*"
    End Sub

    Private Sub TxtUser_TextChanged(sender As Object, e As EventArgs) Handles TxtUser.TextChanged
        ' Input validation can be added here if needed
    End Sub

    Private Sub TxtPass_TextChanged(sender As Object, e As EventArgs) Handles TxtPass.TextChanged
        ' Input validation can be added here if needed
    End Sub

    Private Sub TxtUser_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtUser.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            TxtPass.Focus()
        End If
    End Sub

    Private Sub TxtPass_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtPass.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            BtnLogin.PerformClick()
        End If
    End Sub

    Private Sub BtnLogin_Click(sender As Object, e As EventArgs) Handles BtnLogin.Click
        If String.IsNullOrEmpty(TxtUser.Text) OrElse String.IsNullOrEmpty(TxtPass.Text) Then
            MessageBox.Show("Please enter both username and password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Using conn As New OleDbConnection(connString)
            Try
                conn.Open()
                ' Updated query to join Login and UserInfo tables
                Dim query As String = "SELECT l.UserID, l.Username, l.UserRole, u.FirstName, u.LastName, u.Email " & _
                                    "FROM Login l INNER JOIN UserInfo u ON l.UserID = u.UserID " & _
                                    "WHERE l.Username = ? AND l.Password = ?"
                
                Using cmd As New OleDbCommand(query, conn)
                    cmd.Parameters.AddWithValue("?", TxtUser.Text.Trim())
                    cmd.Parameters.AddWithValue("?", TxtPass.Text.Trim())

                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            ' Store user details in shared variables
                            LoggedInUserID = reader("UserID").ToString()
                            LoggedInFirstName = reader("FirstName").ToString()
                            LoggedInLastName = reader("LastName").ToString()
                            LoggedInUsername = TxtUser.Text.Trim()
                            LoggedInUserRole = reader("UserRole").ToString()

                            ' Navigate to respective dashboards
                            Select Case LoggedInUserRole.ToLower()
                                Case "admin"
                                    Dim adminForm As New AdminDsb()
                                    Me.Hide()
                                    adminForm.Show()
                                Case "user"
                                    Dim userForm As New UserDsb()
                                    Me.Hide()
                                    userForm.Show()
                                Case Else
                                    MessageBox.Show("Invalid user role.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End Select
                        Else
                            MessageBox.Show("Invalid username or password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            TxtPass.Clear() ' Clear password field for security
                            TxtPass.Focus() ' Set focus to password field for convenience
                        End If
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Database error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles BtnSignUp.Click
        Dim SignUpForm As New SignUp()
        Me.Hide()
        SignUpForm.Show()

    End Sub
End Class
