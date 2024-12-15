open System
open System.Drawing
open System.Windows.Forms
open MySql.Data.MySqlClient

let ResetText (textbox: System.Windows.Forms.TextBox) =
    textbox.Text <- ""
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

let studentDetailsForm (parentForm: Form) =
    let form = new Form(Text = "Students", Width = 1500, Height = 1000, BackColor = Color.LightBlue)
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

    let btnAdd = new Button(Text = "Add", Top = 350, Left = 40, Width = 150, Height = 40,BackColor = Color.LightCoral, Font = commonFont)
    let btnEdit = new Button(Text = "Edit", Top = 350, Left = 220, Width = 150, Height = 40, BackColor = Color.LightCoral, Font = commonFont)
    let btnDelete = new Button(Text = "Delete", Top = 350, Left = 400, Width = 150, Height = 40, BackColor = Color.LightCoral, Font = commonFont)
    let btnExit = new Button(Text = "Exit", Top = 420, Left = 220, Width = 150, Height = 40, BackColor = Color.LightCoral, Font = commonFont)


   
    let dgvStudents = new DataGridView(Top = 50, Left = 600, Width = 850, Height = 900)
    dgvStudents.Font <- commonFont
    dgvStudents.AutoSizeColumnsMode <- DataGridViewAutoSizeColumnsMode.Fill
    dgvStudents.ColumnHeadersHeightSizeMode <- DataGridViewColumnHeadersHeightSizeMode.AutoSize
    dgvStudents.ReadOnly <- true

    let query1 = "SELECT ID FROM users WHERE user_type = 'student'"
    let studentIDs = executeQuery connectionString query1
    let autoCompleteCollection = new AutoCompleteStringCollection()
    for row in studentIDs.Rows do
        let studentID = row.["ID"].ToString()
        autoCompleteCollection.Add(studentID)|>ignore

    txtID.KeyPress.Add(fun e ->
        if not (Char.IsDigit(e.KeyChar) || e.KeyChar = '\b') then  
            e.Handled <- true 
    )

    txtID.AutoCompleteMode <- AutoCompleteMode.SuggestAppend
    txtID.AutoCompleteSource <- AutoCompleteSource.CustomSource
    txtID.AutoCompleteCustomSource <- autoCompleteCollection


    let loadData (connectionString: string) =
        let query =  """SELECT u.ID, u.username, s.Name FROM users u JOIN students s ON u.ID = s.ID WHERE u.user_type = 'student';"""

        let data = executeQuery connectionString query 
        dgvStudents.DataSource <- data

   
    loadData connectionString


    btnAdd.Click.Add(fun _ -> 
        let id = txtID.Text
        let name = txtName.Text  
        let username = txtUsername.Text
        let password = txtPassword.Text
        if not (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password)) then
            addStudent id username password name 
            autoCompleteCollection.Add(id) |> ignore
            txtID.AutoCompleteCustomSource <- autoCompleteCollection
            loadData connectionString
            ResetText(txtID)
            ResetText(txtName)
            ResetText(txtUsername)
            ResetText(txtPassword)
    )

    btnEdit.Click.Add(fun _ -> 
        let id = txtID.Text
        let name = txtName.Text  
        let username = txtUsername.Text
        let password = txtPassword.Text
        if not (String.IsNullOrEmpty(id)) then
            editStudent id name username password
            loadData connectionString
            ResetText(txtID)
            ResetText(txtName)
            ResetText(txtUsername)
            ResetText(txtPassword)
    )

    btnDelete.Click.Add(fun _ -> 
        let id = txtID.Text
        if not (String.IsNullOrEmpty(id)) then
            deleteStudent id
            autoCompleteCollection.Remove(id) |> ignore
            txtID.AutoCompleteCustomSource <- autoCompleteCollection
            loadData connectionString
            ResetText(txtID)
            ResetText(txtName)
            ResetText(txtUsername)
            ResetText(txtPassword)
    )

    btnExit.Click.Add(fun _ -> 
        form.Close()
        parentForm.Show()
    )

    form.Controls.AddRange([| lblID; txtID; lblName; txtName; lblUsername; txtUsername; lblPassword; txtPassword; btnAdd; btnEdit; btnDelete; btnExit;dgvStudents |])
    form.ShowDialog() |> ignore


let superUserPanelForm (parentForm: Form) =
    let form = new Form(Text = "SuperUser Panel", Width = 600, Height = 350, BackColor = Color.LightBlue)
    form.FormBorderStyle <- FormBorderStyle.Sizable 
    form.StartPosition <- FormStartPosition.CenterScreen

    let commonFont = new Font("Arial", 12.0f, FontStyle.Regular)

    let btnAddStudent = new Button(Text = "Student",Top = 50, Left = 180, Width = 250,Height = 40, Font = commonFont, BackColor = Color.LightCoral)
    let btnAddInstructor = new Button(Text = "Instructor", Top = 100,Left = 180, Width = 250,Height = 40, Font = commonFont, BackColor = Color.LightCoral)
    let btnAddcourse = new Button(Text = "Course", Top = 150,Left = 180, Width = 250,Height = 40, Font = commonFont, BackColor = Color.LightCoral)
    let btnExit = new Button(Text = "Log Out", Top = 200,Left = 180, Width = 250,Height = 40, Font = commonFont, BackColor = Color.LightCoral)

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

    btnExit.Click.Add(fun _ ->
    form.Close()
    parentForm.Show())

    form.Controls.AddRange([| btnAddStudent; btnAddInstructor;btnAddcourse; btnExit |])



    form.ShowDialog() |> ignore
