Imports System.Data.SQLite
Imports System.Reflection

Public Class Settings

    Public Shared Property scanInterval As Integer = 5
    Public Shared Property email As String = String.Empty
    Public Shared Property lastAlert As String = String.Empty

    Public Shared Sub loadSettings()
        Try
            Dim myPropertyInfo() As PropertyInfo
            Dim myType As Type = GetType(Settings)
            myPropertyInfo = myType.GetProperties()
            Using connection As New SQLiteConnection("Data Source=" & Database.databaseName & ";Version=3")
                connection.Open()
                Using cmd As New SQLiteCommand(connection)
                    cmd.CommandText = "Select * from settings"
                    Dim reader = cmd.ExecuteReader()
                    While reader.Read
                        Dim value = reader("value").ToString
                        Dim setting = reader("setting").ToString
                        For Each thisProperty In myPropertyInfo
                            If setting.ToLower = thisProperty.Name.ToLower Then
                                If TypeOf thisProperty.GetValue(Nothing, Nothing) Is Integer Then
                                    Dim result As Integer
                                    Integer.TryParse(value, result)
                                    thisProperty.SetValue(Nothing, result, Nothing)
                                ElseIf TypeOf thisProperty.GetValue(Nothing, Nothing) Is Boolean Then
                                    Dim result As Boolean
                                    Boolean.TryParse(value, result)
                                    thisProperty.SetValue(Nothing, result, Nothing)
                                Else
                                    thisProperty.SetValue(Nothing, value, Nothing)
                                End If
                                Exit For
                            End If
                        Next
                    End While
                    reader.Close()
                End Using
                connection.Close()
            End Using
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try

    End Sub

    Public Shared Sub saveSettings(Optional ByVal specificSetting As String = Nothing)
        If specificSetting Is Nothing Then
            Try
                Dim myPropertyInfo() As PropertyInfo
                Dim myType As Type = GetType(Settings)
                myPropertyInfo = myType.GetProperties()
                Using connection As New SQLiteConnection("Data Source=" & Database.databaseName & ";Version=3")
                    connection.Open()
                    Using cmd As New SQLiteCommand(connection)
                        For Each thisProperty In myPropertyInfo
                            Dim value = thisProperty.GetValue(Nothing, Nothing)
                            Dim setting = thisProperty.Name
                            cmd.CommandText = "SELECT * FROM settings where setting = @setting"
                            cmd.Prepare()
                            cmd.Parameters.AddWithValue("@setting", setting)
                            Dim reader = cmd.ExecuteReader()
                            If reader.HasRows Then
                                reader.Close()
                                cmd.CommandText = "UPDATE settings set value = @value where setting = @setting"
                            Else
                                reader.Close()
                                cmd.CommandText = "INSERT into settings(setting, value) values (@setting, @value)"
                            End If
                            cmd.Prepare()
                            cmd.Parameters.AddWithValue("@setting", setting)
                            cmd.Parameters.AddWithValue("@value", value)
                            cmd.ExecuteNonQuery()
                        Next
                    End Using
                    connection.Close()
                End Using
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try
        Else
            Try
                Using connection As New SQLiteConnection("Data Source=" & Database.databaseName & ";Version=3")
                    connection.Open()
                    Using cmd As New SQLiteCommand(connection)
                        Dim theProperty = GetType(Settings).GetProperty(specificSetting)
                        If theProperty IsNot Nothing Then
                            Dim value = theProperty.GetValue(Nothing, Nothing)
                            cmd.CommandText = "SELECT * FROM settings where setting = @setting"
                            cmd.Prepare()
                            cmd.Parameters.AddWithValue("@setting", specificSetting)
                            Dim reader = cmd.ExecuteReader()
                            If reader.HasRows Then
                                reader.Close()
                                cmd.CommandText = "UPDATE settings set value = @value where setting = @setting"
                            Else
                                reader.Close()
                                cmd.CommandText = "INSERT into settings(setting, value) values (@setting, @value)"
                            End If
                            cmd.Prepare()
                            cmd.Parameters.AddWithValue("@setting", specificSetting)
                            cmd.Parameters.AddWithValue("@value", value)
                            cmd.ExecuteNonQuery()
                        End If                      
                    End Using
                    connection.Close()
                End Using
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try
        End If
    End Sub


End Class
