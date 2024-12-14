open System
open System.Drawing
open System.Windows.Forms
open MySql.Data.MySqlClient


let connectionString = "Server=localhost;Port=3308;Database=sms;User ID=root;Password=;"

let addStudent id username password name =
    use connection = new MySqlConnection(connectionString)
    connection.Open()

    let queryUser = "INSERT INTO users (ID, username, password, user_type) VALUES (@id, @username, @password, 'student')"
    use commandUser = new MySqlCommand(queryUser, connection)
    commandUser.Parameters.AddWithValue("@id", id) |> ignore
    commandUser.Parameters.AddWithValue("@username", username) |> ignore
    commandUser.Parameters.AddWithValue("@password", password) |> ignore  

    try
        commandUser.ExecuteNonQuery() |> ignore

        let queryStudent = "INSERT INTO students (ID, name) VALUES (@id, @name)"
        use commandStudent = new MySqlCommand(queryStudent, connection)
        commandStudent.Parameters.AddWithValue("@id", id) |> ignore
        commandStudent.Parameters.AddWithValue("@name", name) |> ignore
        commandStudent.ExecuteNonQuery() |> ignore

        MessageBox.Show($"Student with ID '{id}' and name '{name}' added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
    with ex -> 
        MessageBox.Show($"Error adding student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
let editStudent id name username password =
    use connection = new MySqlConnection(connectionString)
    connection.Open()

    let mutable studentUpdates = []
    let mutable userUpdates = []

    if not (String.IsNullOrEmpty(name)) then
        studentUpdates <- ("name = @name") :: studentUpdates

    if not (String.IsNullOrEmpty(username)) then
        userUpdates <- ("username = @username") :: userUpdates

    if not (String.IsNullOrEmpty(password)) then
        userUpdates <- ("password = @password") :: userUpdates

    let updateStudentQuery =
        if studentUpdates.Length > 0 then
            "UPDATE students SET " + String.Join(", ", studentUpdates) + " WHERE ID = @id"
        else
            ""

    let updateUserQuery =
        if userUpdates.Length > 0 then
            "UPDATE users SET " + String.Join(", ", userUpdates) + " WHERE ID = @id"
        else
            ""

    try
        if updateStudentQuery <> "" then
            use studentCommand = new MySqlCommand(updateStudentQuery, connection)
            if not (String.IsNullOrEmpty(name)) then studentCommand.Parameters.AddWithValue("@name", name) |> ignore
            studentCommand.Parameters.AddWithValue("@id", id) |> ignore
            studentCommand.ExecuteNonQuery() |> ignore

        if updateUserQuery <> "" then
            use userCommand = new MySqlCommand(updateUserQuery, connection)
            if not (String.IsNullOrEmpty(username)) then userCommand.Parameters.AddWithValue("@username", username) |> ignore
            if not (String.IsNullOrEmpty(password)) then userCommand.Parameters.AddWithValue("@password", password) |> ignore
            userCommand.Parameters.AddWithValue("@id", id) |> ignore
            userCommand.ExecuteNonQuery() |> ignore

        MessageBox.Show($"Student with ID {id} updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
    with ex -> MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore

let deleteStudent id =
    use connection = new MySqlConnection(connectionString)
    connection.Open()

    try

        let queryStudent = "DELETE FROM students WHERE ID = @id"
        use commandStudent = new MySqlCommand(queryStudent, connection)
        commandStudent.Parameters.AddWithValue("@id", id) |> ignore
        commandStudent.ExecuteNonQuery() |> ignore
        
        let queryUser = "DELETE FROM users WHERE ID = @id"
        use commandUser = new MySqlCommand(queryUser, connection)
        commandUser.Parameters.AddWithValue("@id", id) |> ignore
        commandUser.ExecuteNonQuery() |> ignore

        MessageBox.Show($"Student with ID {id} deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
    with ex -> MessageBox.Show($"Error deleting student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore

let executeQuery (connectionString: string) (query: string)  =
    let table = new DataTable()
    use connection = new MySqlConnection(connectionString)
    use command = new MySqlCommand(query, connection)

    use adapter = new MySqlDataAdapter(command)
    try
        connection.Open()
        adapter.Fill(table) |> ignore
    with
    | ex -> 
        MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
    table

let superUserPanelForm () =
    let form = new Form(Text = "SuperUser Panel", Width = 600, Height = 350, BackColor = Color.LightBlue)
    form.FormBorderStyle <- FormBorderStyle.Sizable 
    form.StartPosition <- FormStartPosition.CenterScreen

    let commonFont = new Font("Arial", 12.0f, FontStyle.Regular)

    let btnAddStudent = new Button(Text = "Student",Top = 50, Left = 180, Width = 250,Height = 40, Font = commonFont, BackColor = Color.LightCoral)
    let btnAddInstructor = new Button(Text = "Instructor", Top = 100,Left = 180, Width = 250,Height = 40, Font = commonFont, BackColor = Color.LightCoral)
    let btnAddcourse = new Button(Text = "Course", Top = 150,Left = 180, Width = 250,Height = 40, Font = commonFont, BackColor = Color.LightCoral)
    let btnExit = new Button(Text = "Exit", Top = 200,Left = 180, Width = 250,Height = 40, Font = commonFont, BackColor = Color.LightCoral)

    btnAddStudent.Click.Add(fun _ -> 
        form.Hide()
        studentDetailsForm form
    )

    btnAddInstructor.Click.Add(fun _ -> 
        form.Hide() 
        instructorDetailsForm form 
    )

    btnAddcourse.Click.Add(fun _ -> 
        form.Hide()
        addcourseform form
    )

    btnExit.Click.Add(fun _ -> form.Close())

    form.Controls.AddRange([| btnAddStudent; btnAddInstructor;btnAddcourse; btnExit |])



    form.ShowDialog() |> ignore