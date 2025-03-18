Private Sub LblForgotPass_Click(sender As Object, e As EventArgs) Handles LblForgotPass.Click
    Dim username As String = InputBox("Enter your username:", "Password Reset")
    
    If String.IsNullOrEmpty(username) Then Return
    
    Using conn As New OleDbConnection(connString)
        Try
            conn.Open()
            ' First check if user exists and get their email
            Dim query As String = "SELECT Email, FirstName FROM Login WHERE Username = ?"
            Dim cmd As New OleDbCommand(query, conn)
            cmd.Parameters.Add("?", OleDbType.VarChar).Value = username.Trim()
            
            Using reader As OleDbDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    Dim email As String = reader("Email").ToString()
                    Dim firstName As String = reader("FirstName").ToString()
                    
                    ' Generate temporary password
                    Dim tempPassword As String = GenerateTemporaryPassword()
                    
                    ' Update password in database - fixed syntax for OleDb
                    Dim updateQuery As String = "UPDATE Login SET [Password] = ? WHERE [Username] = ?"
                    Using updateCmd As New OleDbCommand(updateQuery, conn)
                        updateCmd.Parameters.Add("?", OleDbType.VarChar).Value = tempPassword
                        updateCmd.Parameters.Add("?", OleDbType.VarChar).Value = username
                        updateCmd.ExecuteNonQuery()
                    End Using

                    ' Send email with new password
                    If SendPasswordResetEmail(email, firstName, tempPassword) Then
                        MessageBox.Show("A temporary password has been sent to your email address.",
                                      "Password Reset", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBox.Show("Failed to send email. Please contact support.", 
                                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Else
                    MessageBox.Show("Username not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error resetting password: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Using
End Sub

Private Function IsValidEmail(email As String) As Boolean
    Try
        Dim addr = New MailAddress(email)
        Return addr.Address = email
    Catch
        Return False
    End Try
End Function

Private Function SendPasswordResetEmail(toEmail As String, firstName As String, tempPassword As String) As Boolean
    Try
        ' Validate email first
        If Not IsValidEmail(toEmail) Then
            MessageBox.Show("Invalid email address format.", "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        ' Create the email message
        Dim mail As New MailMessage()
        mail.From = New MailAddress("youremail@example.com")
        mail.To.Add(New MailAddress(toEmail))
        mail.Subject = "Password Reset - SystemName"
        mail.Body = $"Dear {firstName}," & vbCrLf & vbCrLf &
                   "You have requested a password reset for your SystemName account." & vbCrLf &
                   $"Your temporary password is: {tempPassword}" & vbCrLf & vbCrLf &
                   "Please change your password after logging in." & vbCrLf & vbCrLf &
                   "If you did not request this password reset, please contact support immediately." & vbCrLf & vbCrLf &
                   "Best regards," & vbCrLf &
                   "SystemName Team"
      
        Dim smtp As New SmtpClient("smtp.gmail.com")
        smtp.Port = 587
        smtp.EnableSsl = True

        smtp.DeliveryMethod = SmtpDeliveryMethod.Network
        smtp.UseDefaultCredentials = False
        smtp.Credentials = New NetworkCredential("youremail@example.com", "16-digit-app-password")

        ' Send the email
        smtp.Send(mail)
        Return True

    Catch ex As Exception
        MessageBox.Show("Error sending email: " & ex.Message, "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return False
    End Try
End Function

Private Function GenerateTemporaryPassword() As String
    Dim chars As String = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789"
    Dim random As New Random()
    Dim result As New String(Enumerable.Repeat(chars, 8) _
                                     .Select(Function(s) s(random.Next(s.Length))) _
                                     .ToArray())
    Return result
End Function
