Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient

Public Class Connection
    Shared cnx As New MySqlConnection

    Private Shared Sub Connect()
        Try
            cnx.ConnectionString = "Server=localhost;Database=topartistasregion;Uid=root;Pwd=admin"
            cnx.Open()
        Catch ex As Exception
            MsgBox("Error al conectar a la base de datos: " & ex.Message)
        End Try
    End Sub

    Public Shared Sub Disconnect()
        Try
            If cnx.State = ConnectionState.Open Then
                cnx.Close()
            End If
        Catch ex As Exception
            MsgBox("Error al desconectar de la base de datos: " & ex.Message)
        End Try
    End Sub

    ' Método para ejecutar una consulta de selección
    Public Shared Function SelectQuery(ByVal query As String) As DataTable
        Dim dt As New DataTable
        Try
            Connect()
            Dim cmd As New MySqlCommand(query, cnx)
            Dim da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        Catch ex As Exception
            MsgBox("Error al ejecutar la consulta: " & ex.Message)
        Finally
            Disconnect()
        End Try
        Return dt
    End Function
    Public Shared Function ExecuteStoredProcedureReader(ByVal procedureName As String, ByVal parameters As MySqlParameter()) As MySqlDataReader
        Dim reader As MySqlDataReader = Nothing
        Try
            Connect()
            Dim cmd As New MySqlCommand(procedureName, cnx)
            cmd.CommandType = CommandType.StoredProcedure

            ' Agregar parámetros si los hay
            If parameters IsNot Nothing Then
                For Each param As MySqlParameter In parameters
                    cmd.Parameters.Add(param)
                Next
            End If

            reader = cmd.ExecuteReader()

            ' Verificar si hay filas antes de cerrar la conexión
            If Not reader.HasRows Then
                ' Si no hay filas, cerrar el lector y la conexión
                reader.Close()
                Disconnect()
            End If
        Catch ex As Exception
            MsgBox("Error al ejecutar el stored procedure: " & ex.Message)
            Throw
        End Try
        Return reader
    End Function

    Public Shared Function ExecuteStoredProcedure(ByVal storedProcedureName As String, ByVal parameters As MySqlParameter()) As DataTable
        Dim dt As New DataTable
        Try
            Connect()
            Dim cmd As New MySqlCommand()
            cmd.Connection = cnx
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = storedProcedureName

            If parameters IsNot Nothing AndAlso parameters.Length > 0 Then
                For Each param As MySqlParameter In parameters
                    cmd.Parameters.Add(param)
                Next
            End If

            Dim da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        Catch ex As Exception
            MsgBox("Error al ejecutar el stored procedure: " & ex.Message)
        Finally
            Disconnect()
        End Try
        Return dt
    End Function
End Class
