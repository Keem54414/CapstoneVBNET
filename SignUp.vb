Imports System.Data.OleDb

Public Class SignUp
    Private connString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\oz\source\repos\CRop\CRop\PastryPouchDB.accdb;"

    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TxtPass.PasswordChar = "*"
    End Sub
    Private Sub BtnSignUp_Click(sender As Object, e As EventArgs) Handles BtnSignUp.Click
        ' Validate input fields
        If String.IsNullOrEmpty(TxtFName.Text) OrElse
           String.IsNullOrEmpty(TxtLName.Text) OrElse
           String.IsNullOrEmpty(TxtNum.Text) OrElse
           String.IsNullOrEmpty(TxtUser.Text) OrElse
           String.IsNullOrEmpty(TxtPass.Text) Then
            MessageBox.Show("Please fill in all fields.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Using conn As New OleDbConnection(connString)
            Try
                conn.Open()

                ' Check if username already exists
                Dim checkCmd As New OleDbCommand("SELECT COUNT(*) FROM Login WHERE Username = ?", conn)
                checkCmd.Parameters.Add("Username", OleDbType.VarChar).Value = TxtUser.Text.Trim()
                Dim userCount As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                If userCount > 0 Then
                    MessageBox.Show("Username already exists. Please choose a different username.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                ' Insert new user - matches exact database structure (excluding UserID which is AutoNumber)
                Dim sql As String = "INSERT INTO Login ([Username], [Password], [UserRole], [FirstName], [LastName], [PhoneNum]) VALUES (?, ?, ?, ?, ?, ?)"
                Dim cmd As New OleDbCommand(sql, conn)

                ' Add parameters in exact database column order
                With cmd.Parameters
                    .AddWithValue("?", TxtUser.Text.Trim())    ' Username
                    .AddWithValue("?", TxtPass.Text.Trim())    ' Password
                    .AddWithValue("?", "user")                 ' UserRole
                    .AddWithValue("?", TxtFName.Text.Trim())   ' FirstName
                    .AddWithValue("?", TxtLName.Text.Trim())   ' LastName
                    .AddWithValue("?", TxtNum.Text.Trim())     ' PhoneNum
                End With

                cmd.ExecuteNonQuery()
                MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Open login form and close signup form
                Dim loginForm As New Login()
                loginForm.Show()
                Me.Close()

            Catch ex As Exception
                MessageBox.Show("Database error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TxtFName.TextChanged
        ' Input validation can be added here
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        ' Handle label click if needed
    End Sub

    Private Sub BtnSignUp_Click_1(sender As Object, e As EventArgs) Handles BtnSignUp.Click

    End Sub
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If TxtPass.PasswordChar = "*" Then
            TxtPass.PasswordChar = ""
            PictureBox1.Image = My.Resources.eye_open  ' Assign image
            PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage  ' Stretch the image
        Else
            TxtPass.PasswordChar = "*"
            PictureBox1.Image = My.Resources.eye_closed  ' Assign image
            PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage  ' Stretch the image
        End If
    End Sub

    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        Dim LoginForm As New Login()
        Me.Hide()
        LoginForm.Show()
    End Sub
End Class