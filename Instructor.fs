open System
open System.Drawing
open System.Windows.Forms
open MySql.Data.MySqlClient


let connectionString = "Server=localhost;Port=3308;Database=sms;User ID=root;Password=;"


//Instructor Functions Section
let addInstructor id username password name =
    use connection = new MySqlConnection(connectionString)
    connection.Open()

    let queryUser = "INSERT INTO users (ID, username, password, user_type) VALUES (@id, @username, @password, 'instructor')"
    use commandUser = new MySqlCommand(queryUser, connection)
    commandUser.Parameters.AddWithValue("@id", id) |> ignore
    commandUser.Parameters.AddWithValue("@username", username) |> ignore
    commandUser.Parameters.AddWithValue("@password", password) |> ignore
    
    try
        commandUser.ExecuteNonQuery() |> ignore

        let queryInstructor = "INSERT INTO instructor (ID, name) VALUES (@id, @name)"
        use commandInstructor = new MySqlCommand(queryInstructor, connection)
        commandInstructor.Parameters.AddWithValue("@id", id) |> ignore
        commandInstructor.Parameters.AddWithValue("@name", name) |> ignore
        commandInstructor.ExecuteNonQuery() |> ignore

        MessageBox.Show($"Instructor with ID '{id}' added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
    with ex -> 
        MessageBox.Show($"Error adding instructor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore

let editInstructor id name username password =
    use connection = new MySqlConnection(connectionString)
    connection.Open()

    let mutable instructorUpdates = []
    let mutable userUpdates = []

    if not (String.IsNullOrEmpty(name)) then
        instructorUpdates <- ("name = @name") :: instructorUpdates

    if not (String.IsNullOrEmpty(username)) then
        userUpdates <- ("username = @username") :: userUpdates

    if not (String.IsNullOrEmpty(password)) then
        userUpdates <- ("password = @password") :: userUpdates

    let updateInstructorQuery =
        if instructorUpdates.Length > 0 then
            "UPDATE instructor SET " + String.Join(", ", instructorUpdates) + " WHERE ID = @id"
        else
            ""

    let updateUserQuery =
        if userUpdates.Length > 0 then
            "UPDATE users SET " + String.Join(", ", userUpdates) + " WHERE ID = @id"
        else
            ""

    try
        if updateInstructorQuery <> "" then
            use instructorCommand = new MySqlCommand(updateInstructorQuery, connection)
            if not (String.IsNullOrEmpty(name)) then instructorCommand.Parameters.AddWithValue("@name", name) |> ignore
            instructorCommand.Parameters.AddWithValue("@id", id) |> ignore
            instructorCommand.ExecuteNonQuery() |> ignore

        if updateUserQuery <> "" then
            use userCommand = new MySqlCommand(updateUserQuery, connection)
            if not (String.IsNullOrEmpty(username)) then userCommand.Parameters.AddWithValue("@username", username) |> ignore
            if not (String.IsNullOrEmpty(password)) then userCommand.Parameters.AddWithValue("@password", password) |> ignore
            userCommand.Parameters.AddWithValue("@id", id) |> ignore
            userCommand.ExecuteNonQuery() |> ignore

        MessageBox.Show($"Instructor with ID {id} updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
    with ex -> 
        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore

let deleteInstructor id =
    use connection = new MySqlConnection(connectionString)
    connection.Open()

    let queryInstructor = "DELETE FROM instructor WHERE ID = @id"
    use commandInstructor = new MySqlCommand(queryInstructor, connection)
    commandInstructor.Parameters.AddWithValue("@id", id) |> ignore

    let queryUser = "DELETE FROM users WHERE ID = @id"
    use commandUser = new MySqlCommand(queryUser, connection)
    commandUser.Parameters.AddWithValue("@id", id) |> ignore

    try
        commandInstructor.ExecuteNonQuery() |> ignore
        commandUser.ExecuteNonQuery() |> ignore
        MessageBox.Show($"Instructor with ID {id} deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
    with ex -> 
        MessageBox.Show($"Error deleting instructor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore


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


let instructorDetailsForm (parentForm: Form) =
    
    let form = new Form(Text = "Instructors", Width = 1500, Height = 1000, BackColor = Color.LightBlue)
    form.FormBorderStyle <- FormBorderStyle.Sizable
    form.StartPosition <- FormStartPosition.CenterScreen

    let commonFont = new Font("Arial", 12.0f, FontStyle.Regular)

    let lblID = new Label(Text = "ID", Top = 50, Left = 70, ForeColor = Color.White, Font = commonFont)
    let txtID = new TextBox(Top = 50, Left = 180, Width = 250, Font = commonFont)
    let lblName = new Label(Text = "Name", Top = 130, Left = 70, ForeColor = Color.White, Font = commonFont)
    let txtName = new TextBox(Top = 130, Left = 180, Width = 250, Font = commonFont)
    let lblUsername = new Label(Text = "Username", Top = 210, Left = 70, ForeColor = Color.White, Font = commonFont)
    let txtUsername = new TextBox(Top = 210, Left = 180, Width = 250, Font = commonFont)
    let lblPassword = new Label(Text = "Password", Top = 290, Left = 70, ForeColor = Color.White, Font = commonFont)
    let txtPassword = new TextBox(Top = 290, Left = 180, Width = 250, Font = commonFont)

    let btnAdd = new Button(Text = "Add ", Top = 350, Left = 40, Width = 150, Height = 40,BackColor = Color.LightCoral, Font = commonFont)
    let btnEdit = new Button(Text = "Edit ", Top = 350, Left = 220, Width = 150, Height = 40, BackColor = Color.LightCoral, Font = commonFont)
    let btnDelete = new Button(Text = "Delete ", Top = 350, Left = 400, Width = 150, Height = 40, BackColor = Color.LightCoral, Font = commonFont)
    let btnExit = new Button(Text = "Exit", Top = 420, Left = 220, Width = 150, Height = 40, BackColor = Color.LightCoral, Font = commonFont)
    
    let dgvStudents = new DataGridView(Top = 50, Left = 600, Width = 850, Height = 900)
    dgvStudents.Font <- commonFont
    dgvStudents.AutoSizeColumnsMode <- DataGridViewAutoSizeColumnsMode.Fill
    dgvStudents.ColumnHeadersHeightSizeMode <- DataGridViewColumnHeadersHeightSizeMode.AutoSize
    dgvStudents.ReadOnly <- true

    let loadData (connectionString: string) =
        let query =  """SELECT u.ID, u.username, i.Name FROM users u JOIN instructor i ON u.ID = i.ID WHERE u.user_type = 'instructor';"""

        let data = executeQuery connectionString query 
        dgvStudents.DataSource <- data

   
    loadData connectionString
    


    btnAdd.Click.Add(fun _ -> 
        let id = txtID.Text
        let name = txtName.Text
        let username = txtUsername.Text
        let password = txtPassword.Text
        if not (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password)) then
            addInstructor id username password name
            loadData connectionString
    )

    btnEdit.Click.Add(fun _ -> 
        let id = txtID.Text
        let name = txtName.Text
        let username = txtUsername.Text
        let password = txtPassword.Text
        if not (String.IsNullOrEmpty(id)) then
            editInstructor id name username password
            loadData connectionString
    )

    btnDelete.Click.Add(fun _ -> 
        let id = txtID.Text
        if not (String.IsNullOrEmpty(id)) then
            deleteInstructor id
            loadData connectionString
    )

    btnExit.Click.Add(fun _ -> 
        form.Close()
        parentForm.Show()
    )

    form.Controls.AddRange([| lblID; txtID; lblName; txtName; lblUsername; txtUsername; lblPassword; txtPassword; btnAdd; btnEdit; btnDelete; btnExit ;dgvStudents|])

    form.ShowDialog() |> ignore