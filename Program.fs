open Npgsql
open Npgsql.FSharp
open ThrowawayDb.Postgres

let createTestDatabase() =
    Sql.host "localhost"
    |> Sql.port 5432
    |> Sql.username "postgres"
    |> Sql.password "postgres"
    |> Sql.formatConnectionString
    |> ThrowawayDatabase.Create

let parameters() =
    use database = createTestDatabase()

    // Add users table
    database.ConnectionString
    |> Sql.connect
    |> Sql.query "CREATE TABLE users (user_id bigserial primary key, username text not null, roles text[])"
    |> Sql.executeNonQuery
    |> ignore

    let query = "INSERT INTO users (username, roles) VALUES (@username, @roles)"
    use connection = new NpgsqlConnection(database.ConnectionString)
    connection.Open()
    use cmd = new NpgsqlCommand(query, connection)
    NpgsqlCommandBuilder.DeriveParameters(cmd)
    cmd.Parameters

for parameter in parameters() do
    printfn "%s: %s (Nullable = %b)"
        parameter.ParameterName
        parameter.PostgresType.Name
        parameter.IsNullable