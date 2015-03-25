Imports System.Data.SQLite
Imports System.IO

Public Class Database

    Private Shared instance As Database
    Private Property connection As SQLiteConnection
    Public Shared ReadOnly databaseName As String = My.Application.Info.DirectoryPath & "\database.sqlite"

    Public Shared Function getInstance() As Database
        If instance Is Nothing Then
            instance = Create()
        End If
        Return instance
    End Function

    Private Shared Function Create() As Database
        Dim db As New Database
        Dim createdFile As Boolean = False
        If Not System.IO.File.Exists(databaseName) Then
            Try
                SQLiteConnection.CreateFile(databaseName)
                createdFile = True
            Catch ex1 As Exception
                Debug.WriteLine(ex1.Message)
            End Try
        End If
        Try
            db.connection = New SQLiteConnection("Data Source=" & databaseName & ";Version=3")
            db.connection.Open()

            If createdFile Then
                Dim cmdString As String = "CREATE TABLE vendors(_id INTEGER PRIMARY KEY AUTOINCREMENT,mac_address TEXT NOT NULL,name TEXT NOT NULL,UNIQUE(mac_address))"
                Dim command As New SQLiteCommand(cmdString, db.connection)
                command.ExecuteNonQuery()

                cmdString = "CREATE TABLE networks(_id INTEGER PRIMARY KEY AUTOINCREMENT,network_name TEXT NOT NULL,monitor INTEGER NOT NULL)"
                command = New SQLiteCommand(cmdString, db.connection)
                command.ExecuteNonQuery()

                cmdString = "CREATE TABLE trustedIPs(_id INTEGER PRIMARY KEY AUTOINCREMENT,ip_address TEXT NOT NULL,network_id INTEGER NOT NULL,UNIQUE(ip_address, network_id))"
                command = New SQLiteCommand(cmdString, db.connection)
                command.ExecuteNonQuery()

                cmdString = "CREATE TABLE trustedMACs(_id INTEGER PRIMARY KEY AUTOINCREMENT,mac_address TEXT NOT NULL,network_id INTEGER NOT NULL,UNIQUE(mac_address, network_id))"
                command = New SQLiteCommand(cmdString, db.connection)
                command.ExecuteNonQuery()

                cmdString = "CREATE TABLE trustedHostnames(_id INTEGER PRIMARY KEY AUTOINCREMENT,host_name TEXT NOT NULL,network_id INTEGER NOT NULL,UNIQUE(host_name, network_id))"
                command = New SQLiteCommand(cmdString, db.connection)
                command.ExecuteNonQuery()

                cmdString = "CREATE TABLE nodes(_id INTEGER PRIMARY KEY AUTOINCREMENT,ip_address TEXT NOT NULL,mac_address TEXT NOT NULL,host_name TEXT NOT NULL,network_id INTEGER NOT NULL,UNIQUE(ip_address, mac_address, host_name, network_id))"
                command = New SQLiteCommand(cmdString, db.connection)
                command.ExecuteNonQuery()

                cmdString = "CREATE TABLE settings(_id INTEGER PRIMARY KEY AUTOINCREMENT,setting TEXT NOT NULL, value TEXT NOT NULL, UNIQUE(setting))"
                command = New SQLiteCommand(cmdString, db.connection)
                command.ExecuteNonQuery()

                Settings.saveSettings()

            Else
                'UPGRADE DB
            End If

            'db.connection.Close()
        Catch ex As Exception
            db = Nothing
        End Try
        Return db
    End Function

    Public Sub CloseConnection()
        Try
            connection.Close()
        Catch ex As Exception
            writeError(ex)
        End Try
    End Sub

    Public Function getNetwork(name As String) As Network
        Dim toReturn As Network = Nothing
        Try
            Using cmd As New SQLiteCommand(connection)
                cmd.CommandText = "Select * from networks where network_name = @name"
                cmd.Prepare()
                cmd.Parameters.AddWithValue("@name", name)
                Dim reader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    toReturn = New Network(reader("network_name"))
                    toReturn.id = reader("_id")
                    toReturn.monitor = reader("monitor")
                End If
                reader.Close()
            End Using
        Catch ex As Exception
            writeError(ex)
        End Try
        Return toReturn
    End Function

    Public Function upsertNetwork(network As Network) As Boolean
        Dim toReturn As Boolean = False
        Try
            Dim existingNetwork As Network = getNetwork(network.name)
            Using cmd As New SQLiteCommand(connection)
                If existingNetwork Is Nothing Then
                    cmd.CommandText = "INSERT into networks(network_name, monitor) values(@NAME, @MONITOR);SELECT last_insert_rowID()"
                Else
                    cmd.CommandText = "UPDATE networks set network_name = @NAME, monitor = @MONITOR where _id = @ID"
                End If
                cmd.Prepare()
                cmd.Parameters.AddWithValue("@NAME", network.name)
                cmd.Parameters.AddWithValue("@MONITOR", network.monitor)
                cmd.Parameters.AddWithValue("@ID", network.id)
                Dim resultID As Long = CType(cmd.ExecuteScalar(), Long)
                writeError(New Exception("Found network with id " & resultID))
                network.id = If(resultID <> network.id, resultID, network.id)
                toReturn = True
            End Using
        Catch ex As Exception
            writeError(ex)
        End Try
        Return toReturn
    End Function

    Public Function upsertNode(node As NetworkNode) As Boolean
        Dim success As Boolean = False
        Try
            Using cmd As New SQLiteCommand(connection)
                cmd.CommandText = "Select * from networks where _id = @id"
                cmd.Prepare()
                cmd.Parameters.AddWithValue("@id", node.id)
                Dim reader As SQLiteDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Close()
                    cmd.CommandText = "UPDATE nodes set ip_address = @IP, set mac_address = @MAC, set host_name = @HOST, set network_id = @NET_ID where _id = @ID"
                Else
                    reader.Close()
                    cmd.CommandText = "INSERT into nodes(ip_address, mac_address, host_name, network_id) values(@IP, @MAC, @HOST, @NET_ID);SELECT last_insert_rowID()"
                End If
                cmd.Prepare()
                cmd.Parameters.AddWithValue("@IP", node.ipAddress)
                cmd.Parameters.AddWithValue("@MAC", node.macAddress)
                cmd.Parameters.AddWithValue("@HOST", node.hostName)
                cmd.Parameters.AddWithValue("@NET_ID", node.networkID)
                cmd.Parameters.AddWithValue("@ID", node.id)
                Dim resultID As Long = CType(cmd.ExecuteScalar(), Long)
                node.id = If(resultID <> node.id, resultID, node.id)
                success = True
            End Using
        Catch ex As Exception
            writeError(ex)
        End Try
        Return success
    End Function

    Public Function setIPTrusted(node As NetworkNode) As Boolean
        Dim success As Boolean = False
        Try
            Using cmd As New SQLiteCommand(connection)
                If node.trustedIP Then
                    cmd.CommandText = "INSERT into trustedIPs(ip_address, network_id) values(@IP, @NET_ID)"
                Else
                    cmd.CommandText = "DELETE FROM trustedIPs where ip_address = @IP and network_id = @NET_ID"
                End If
                cmd.Prepare()
                cmd.Parameters.AddWithValue("@IP", node.ipAddress)
                cmd.Parameters.AddWithValue("@NET_ID", node.networkID)
                cmd.ExecuteNonQuery()
                success = True
            End Using
        Catch ex As Exception
            writeError(ex)
        End Try
        Return success
    End Function

    Public Function setMACTrusted(node As NetworkNode) As Boolean
        Dim success As Boolean = False
        Try
            Using cmd As New SQLiteCommand(connection)
                If node.trustedMAC Then
                    cmd.CommandText = "INSERT into trustedMACs(mac_address, network_id) values(@MAC, @NET_ID)"
                Else
                    cmd.CommandText = "DELETE FROM trustedMACs where mac_address = @MAC and network_id = @NET_ID"
                End If
                cmd.Prepare()
                cmd.Parameters.AddWithValue("@MAC", node.macAddress)
                cmd.Parameters.AddWithValue("@NET_ID", node.networkID)
                cmd.ExecuteNonQuery()
                success = True
            End Using
        Catch ex As Exception
            writeError(ex)
        End Try
        Return success
    End Function

    Public Function setHostNameTrusted(node As NetworkNode) As Boolean
        Dim success As Boolean = False
        Try
            Using cmd As New SQLiteCommand(connection)
                If node.trustedHostName Then
                    cmd.CommandText = "INSERT into trustedHostnames(host_name, network_id) values(@HOST, @NET_ID)"
                Else
                    cmd.CommandText = "DELETE FROM trustedHostnames where host_name = @HOST and network_id = @NET_ID"
                End If
                cmd.Prepare()
                cmd.Parameters.AddWithValue("@HOST", node.hostName)
                cmd.Parameters.AddWithValue("@NET_ID", node.networkID)
                cmd.ExecuteNonQuery()
                success = True
            End Using
        Catch ex As Exception
            writeError(ex)
        End Try
        Return success
    End Function

    Public Function getVendorName(node As NetworkNode) As String
        Dim vendorName As String = String.Empty
        Try
            Using cmd As New SQLiteCommand(connection)
                cmd.CommandText = "Select * from vendors where mac_address like @MAC Order by mac_address Desc"
                cmd.Prepare()
                cmd.Parameters.AddWithValue("@MAC", node.macAddress.Substring(0, 8).Replace("-", ":") & "%")
                Dim reader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    vendorName = reader("name").ToString
                End If
                reader.Close()
            End Using
        Catch ex As Exception
            writeError(ex)
        End Try
        Return vendorName
    End Function

    Public Function getIsIPTrusted(node As NetworkNode) As Boolean
        Dim isTrusted As Boolean = False
        Try
            Using cmd As New SQLiteCommand(connection)
                cmd.CommandText = "Select * from trustedIPs where network_id = @NET_ID and ip_address = @IP"
                cmd.Prepare()
                cmd.Parameters.AddWithValue("@NET_ID", node.networkID)
                cmd.Parameters.AddWithValue("@IP", node.ipAddress)
                Dim reader = cmd.ExecuteReader()
                If reader.HasRows Then
                    isTrusted = True
                End If
                reader.Close()
            End Using
        Catch ex As Exception
            writeError(ex)
        End Try
        Return isTrusted
    End Function

    Public Function getIsMACTrusted(node As NetworkNode) As Boolean
        Dim isTrusted As Boolean = False
        Try
            Using cmd As New SQLiteCommand(connection)
                cmd.CommandText = "Select * from trustedMACs where network_id = @NET_ID and mac_address = @MAC"
                cmd.Prepare()
                cmd.Parameters.AddWithValue("@NET_ID", node.networkID)
                cmd.Parameters.AddWithValue("@MAC", node.macAddress)
                Dim reader = cmd.ExecuteReader()
                If reader.HasRows Then
                    isTrusted = True
                End If
                reader.Close()
            End Using
        Catch ex As Exception
            writeError(ex)
        End Try
        Return isTrusted
    End Function

    Public Function getIsHostNameTrusted(node As NetworkNode) As Boolean
        Dim isTrusted As Boolean = False
        Try
            Using cmd As New SQLiteCommand(connection)
                cmd.CommandText = "Select * from trustedHostnames where network_id = @NET_ID and host_name = @HOST"
                cmd.Prepare()
                cmd.Parameters.AddWithValue("@NET_ID", node.networkID)
                cmd.Parameters.AddWithValue("@HOST", node.hostName)
                Dim reader = cmd.ExecuteReader()
                If reader.HasRows Then
                    isTrusted = True
                End If
                reader.Close()
            End Using
        Catch ex As Exception
            writeError(ex)
        End Try
        Return isTrusted
    End Function

    Public Function getNodeID(node As NetworkNode) As Long
        Dim nodeID As Long = 0
        Try
            Using cmd As New SQLiteCommand(connection)
                cmd.CommandText = "Select * from nodes where ip_address = @IP and mac_address = @MAC and host_name = @HOST and network_id = @NET_ID"
                cmd.Prepare()
                cmd.Parameters.AddWithValue("@IP", node.ipAddress)
                cmd.Parameters.AddWithValue("@MAC", node.macAddress)
                cmd.Parameters.AddWithValue("@HOST", node.hostName)
                cmd.Parameters.AddWithValue("@NET_ID", node.networkID)
                Dim reader = cmd.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    nodeID = CType(reader("_id"), Long)
                End If
                reader.Close()
            End Using
        Catch ex As Exception
            writeError(ex)
        End Try
        Return nodeID
    End Function

    Public Function updateVendors(vendorList As List(Of Vendor)) As Integer
        Dim updated As Integer = 0
        Try
            Using cmd As New SQLiteCommand(connection)
                Using transaction As SQLiteTransaction = connection.BeginTransaction
                    For Each currentVendor In vendorList
                        cmd.CommandText = "INSERT OR IGNORE into vendors(mac_address, name) values(@MAC, @NAME)"
                        cmd.Prepare()
                        cmd.Parameters.AddWithValue("@MAC", currentVendor.hwId)
                        cmd.Parameters.AddWithValue("@NAME", currentVendor.name)
                        Try
                            cmd.ExecuteNonQuery()
                            updated += 1
                        Catch ex As Exception
                            writeError(ex)
                        End Try
                    Next
                    transaction.Commit()
                End Using
            End Using
        Catch ex As Exception
            writeError(ex)
        End Try
        Return updated
    End Function

    Private Sub writeError(theException As Exception)
        Try
            Using writer As New StreamWriter("log.txt")
                writer.WriteLine(DateTime.Now.ToString() & " - " & theException.Message)
            End Using
        Catch ex As Exception

        End Try
    End Sub




End Class
