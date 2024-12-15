open System
open System.Drawing
open System.Windows.Forms
open MySql.Data.MySqlClient
open System.Data



let connectionString = "Server=localhost;Port=3308;Database=sms;User ID=root;Password=;"


let ResetText (textbox: System.Windows.Forms.TextBox) =
    textbox.Text <- ""
//courses functions


let addCourse id course =
    use connection = new MySqlConnection(connectionString)
    connection.Open()

    let querycourse = "INSERT INTO courses (ID, course) VALUES (@id, @course)"
    use commandcourse = new MySqlCommand(querycourse, connection)
    commandcourse.Parameters.AddWithValue("@id", id) |> ignore
    commandcourse.Parameters.AddWithValue("@course", course) |> ignore
 

    try
        commandcourse.ExecuteNonQuery() |> ignore
        MessageBox.Show($"course with ID '{id}' and name '{course}' added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
    with ex -> 
        MessageBox.Show($"Error adding course: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore


let deleteCourse id =
    use connection = new MySqlConnection(connectionString)
    connection.Open()

    try

        let querycourse = "DELETE FROM courses WHERE ID = @id"
        use commandcourse = new MySqlCommand(querycourse, connection)
        commandcourse.Parameters.AddWithValue("@id", id) |> ignore
        commandcourse.ExecuteNonQuery() |> ignore
        
        MessageBox.Show($"course with ID {id} deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
    with ex -> MessageBox.Show($"Error deleting course: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore

//Grades Functions Section
let addGrade studentID courseName grade =
    use connection = new MySqlConnection(connectionString)
    connection.Open()
    try
       
        let queryCheckStudent = "SELECT COUNT(*) FROM users WHERE ID = @studentID"
        use commandCheckStudent = new MySqlCommand(queryCheckStudent, connection)
        commandCheckStudent.Parameters.AddWithValue("@studentID", studentID) |> ignore

        let studentExists =
            match commandCheckStudent.ExecuteScalar() with
            | :? int64 as count -> count > 0L
            | _ -> false

        if studentExists then
            let queryGetCourseID = "SELECT ID FROM courses WHERE course = @courseName"
            use commandGetCourseID = new MySqlCommand(queryGetCourseID, connection)
            commandGetCourseID.Parameters.AddWithValue("@courseName", courseName) |> ignore
            let courseID= commandGetCourseID.ExecuteScalar()

            let queryCheckCourse = "SELECT COUNT(*) FROM students_details WHERE ID = @studentID AND course_id = @courseID"
            use commandCheckCourse = new MySqlCommand(queryCheckCourse, connection)
            commandCheckCourse.Parameters.AddWithValue("@studentID", studentID) |> ignore
            commandCheckCourse.Parameters.AddWithValue("@courseID", courseID) |> ignore

            let courseExists =
                match commandCheckCourse.ExecuteScalar() with
                | :? int64 as count -> count > 0L
                | _ -> false

            if courseExists then
                MessageBox.Show("Course is already entered for that student. If you want to edit the grade, please press Update.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
            else

                let queryInsert = "INSERT INTO students_details (ID, course_id, grades) VALUES (@studentID, @courseID, @grade)"
                use commandInsert = new MySqlCommand(queryInsert, connection)
                commandInsert.Parameters.AddWithValue("@studentID", studentID) |> ignore
                commandInsert.Parameters.AddWithValue("@courseID", courseID) |> ignore
                commandInsert.Parameters.AddWithValue("@grade", grade) |> ignore

                commandInsert.ExecuteNonQuery() |> ignore
                MessageBox.Show($"Course '{courseName}' (ID: {courseID}) and grade '{grade}' added for student ID '{studentID}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
                MessageBox.Show("Grade added successfully!", "Success") |> ignore
        else
            MessageBox.Show("Invalid ID! No student has that ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
    with
    | ex -> 
        MessageBox.Show($"Error adding grade: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore

let updateGrade studentID courseName grade =
    use connection = new MySqlConnection(connectionString)
    try
        connection.Open()

        let queryGetCourseID = "SELECT ID FROM courses WHERE course = @courseName"
        use commandGetCourseID = new MySqlCommand(queryGetCourseID, connection)
        commandGetCourseID.Parameters.AddWithValue("@courseName", courseName) |> ignore
        let courseID = commandGetCourseID.ExecuteScalar()

        let queryCheck = "SELECT COUNT(*) FROM students_details WHERE ID = @studentID AND course_id = @courseID"
        use commandCheck = new MySqlCommand(queryCheck, connection)
        commandCheck.Parameters.AddWithValue("@studentID", studentID) |> ignore
        commandCheck.Parameters.AddWithValue("@courseID", courseID) |> ignore

        let recordExists =
            match commandCheck.ExecuteScalar() with
            | :? int64 as count -> count > 0L
            | _ -> false

        if recordExists then
            let queryUpdate = "UPDATE students_details SET grades = @grade WHERE ID = @studentID AND course_id = @courseID"
            use commandUpdate = new MySqlCommand(queryUpdate, connection)
            commandUpdate.Parameters.AddWithValue("@studentID", studentID) |> ignore
            commandUpdate.Parameters.AddWithValue("@courseID", courseID) |> ignore
            commandUpdate.Parameters.AddWithValue("@grade", grade) |> ignore
            commandUpdate.ExecuteNonQuery() |> ignore
            MessageBox.Show($"Course '{courseName}' and grade updated to '{grade}' for student ID '{studentID}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
            MessageBox.Show("Grade Updated successfully!", "Success") |> ignore
        else
            MessageBox.Show($"No record found for student ID '{studentID}' and course '{courseName}'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore

    with
    | ex -> 
        MessageBox.Show($"Error updating course and grade: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore


let deleteGrade studentID courseName =
    use connection = new MySqlConnection(connectionString)
    try
        connection.Open()

        let queryGetCourseID = "SELECT ID FROM courses WHERE course = @courseName"
        use commandGetCourseID = new MySqlCommand(queryGetCourseID, connection)
        commandGetCourseID.Parameters.AddWithValue("@courseName", courseName) |> ignore
        let courseID = commandGetCourseID.ExecuteScalar()

        let queryCheck = "SELECT COUNT(*) FROM students_details WHERE ID = @studentID AND course_id = @courseID"
        use commandCheck = new MySqlCommand(queryCheck, connection)
        commandCheck.Parameters.AddWithValue("@studentID", studentID) |> ignore
        commandCheck.Parameters.AddWithValue("@courseID", courseID) |> ignore

        let recordExists =
            match commandCheck.ExecuteScalar() with
            | :? int64 as count -> count > 0L
            | _ -> false

        if recordExists then
            let queryDelete = "DELETE FROM students_details WHERE ID = @studentID AND course_id = @courseID"
            use commandDelete = new MySqlCommand(queryDelete, connection)
            commandDelete.Parameters.AddWithValue("@studentID", studentID) |> ignore
            commandDelete.Parameters.AddWithValue("@courseID", courseID) |> ignore

            commandDelete.ExecuteNonQuery() |> ignore
            MessageBox.Show($"Course '{courseName}' and its grade have been deleted for student ID '{studentID}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
            MessageBox.Show("Grade Deleted successfully!", "Success") |> ignore
        else
            MessageBox.Show($"No record found for course '{courseName}' and student ID '{studentID}'.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
    with
    | ex ->
        MessageBox.Show($"Error deleting course and grade: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore


let viewGrades studentID =
    use connection = new MySqlConnection(connectionString)
    try
        connection.Open()

        let query = "SELECT c.course, sd.grades FROM students_details sd JOIN courses c ON sd.course_id = c.ID WHERE sd.ID = @studentID"
        use command = new MySqlCommand(query, connection)
        command.Parameters.AddWithValue("@studentID", studentID) |> ignore

        use reader = command.ExecuteReader()
        let mutable totalGrades = 0.0
        let mutable gradeCount = 0
        let gradesList = 
            [ 
                while reader.Read() do
                    let course = reader.GetString(0)
                    let grade = float(reader.GetFloat(1))
                    totalGrades <- totalGrades + grade
                    gradeCount <- gradeCount + 1
                    yield $"Course: {course}, Grade: {grade}" 
            ]

        let gradesInfo = String.Join("\n", gradesList)
        let averageGrade = 
            if gradeCount > 0 then 
                totalGrades / float gradeCount 
            else 
                0.0

        if gradesInfo = "" then
            MessageBox.Show("No grades published yet.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
        else
            let message = 
                $"{gradesInfo}\n\nAverage Grade: {averageGrade:F2}" 
            MessageBox.Show(message, "Your Grades", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
    with
    | ex ->
        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore

let viewCourseStatistics () =
    use connection = new MySqlConnection(connectionString)
    connection.Open()
    try

        let query = "SELECT c.course AS Course, CASE WHEN MIN(sd.grades) IS NULL THEN NULL ELSE MIN(sd.grades) END AS LowestGrade, CASE WHEN MAX(sd.grades) IS NULL THEN NULL ELSE MAX(sd.grades) END AS HighestGrade, CASE WHEN COUNT(sd.id) = 0 THEN NULL ELSE ROUND(100.0 * SUM(CASE WHEN sd.grades >= 50 THEN 1 ELSE 0 END) / COUNT(sd.id), 2) END AS PassRate, CASE WHEN COUNT(sd.id) = 0 THEN NULL ELSE ROUND(100.0 * SUM(CASE WHEN sd.grades < 50 THEN 1 ELSE 0 END) / COUNT(sd.id), 2) END AS FailRate, CASE WHEN COUNT(sd.id) = 0 THEN NULL ELSE COUNT(sd.id) END AS TotalStudents FROM courses c LEFT JOIN students_details sd ON c.ID = sd.course_id GROUP BY c.course;"

        use command = new MySqlCommand(query, connection)
        use reader = command.ExecuteReader()

        let dataTable = new DataTable()
        dataTable.Load(reader)
        dataTable
    
    with
    | ex -> 
        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        null

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


let addcourseform(parentForm: Form)=
    let form = new Form(Text = "courses", Width = 1500, Height = 1000, BackColor = Color.LightBlue)
    form.FormBorderStyle <- FormBorderStyle.Sizable
    form.StartPosition <- FormStartPosition.CenterScreen

    let commonFont = new Font("Arial", 12.0f, FontStyle.Regular)

    let lblID = new Label(Text = "ID", Top = 50, Left = 70, ForeColor = Color.White, Font = commonFont)
    let txtID = new TextBox(Top = 50, Left = 180, Width = 250, Font = commonFont)
    let lblName = new Label(Text = "Course", Top = 130, Left = 70, ForeColor = Color.White, Font = commonFont)
    let txtName = new TextBox(Top = 130, Left = 180, Width = 250, Font = commonFont)


    let btnAdd = new Button(Text = "Add ", Top = 350, Left = 40, Width = 150, Height = 40,BackColor = Color.LightCoral, Font = commonFont)
    let btnDelete = new Button(Text = "Delete ", Top = 350, Left = 400, Width = 150, Height = 40, BackColor = Color.LightCoral, Font = commonFont)
    let btnExit = new Button(Text = "Exit", Top = 420, Left = 220, Width = 150, Height = 40, BackColor = Color.LightCoral, Font = commonFont)
    
    let dgvStudents = new DataGridView(Top = 50, Left = 600, Width = 850, Height = 900)
    dgvStudents.Font <- commonFont
    dgvStudents.AutoSizeColumnsMode <- DataGridViewAutoSizeColumnsMode.Fill
    dgvStudents.ColumnHeadersHeightSizeMode <- DataGridViewColumnHeadersHeightSizeMode.AutoSize
    dgvStudents.ReadOnly <- true

    let query1 = "SELECT ID FROM courses"
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
        let query = "SELECT DISTINCT ID,course FROM courses"

        let data = executeQuery connectionString query 
        dgvStudents.DataSource <- data

   
    loadData connectionString
    


    btnAdd.Click.Add(fun _ -> 
        let id = txtID.Text
        let name = txtName.Text

        if not (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(name)) then
            addCourse id name

            autoCompleteCollection.Add(id) |> ignore
            txtID.AutoCompleteCustomSource <- autoCompleteCollection
            loadData connectionString
    )


    btnDelete.Click.Add(fun _ -> 
        let id = txtID.Text
        if not (String.IsNullOrEmpty(id)) then
            deleteCourse id
            autoCompleteCollection.Remove(id) |> ignore
            txtID.AutoCompleteCustomSource <- autoCompleteCollection
            loadData connectionString
    )

    btnExit.Click.Add(fun _ -> 
        form.Close()
        parentForm.Show()
    )

    form.Controls.AddRange([| lblID; txtID; lblName; txtName;  btnAdd;  btnDelete; btnExit ;dgvStudents|])

    form.ShowDialog() |> ignore


let addGradesForm (parentForm: Form) =
    use connection = new MySqlConnection(connectionString)
    connection.Open()

    let form = new Form(Text = "Add Grades and Courses", Width = 1500, Height = 1000, BackColor = Color.LightBlue)
    form.FormBorderStyle <- FormBorderStyle.Sizable  
    form.StartPosition <- FormStartPosition.CenterScreen

    let commonFont = new Font("Arial", 12.0f, FontStyle.Regular)

    let lblStudentID = new Label(Text = "ID", Top = 50, Left = 70, ForeColor = Color.White, Font = commonFont)
    let txtStudentID = new TextBox(Top = 50, Left = 180, Width = 250, Font = commonFont)
    let lblCourse = new Label(Text = "Course", Top = 130, Left = 70, ForeColor = Color.White, Font = commonFont)
    let cmbCourse = new ComboBox(Top = 130, Left = 180, Width = 250, Font = commonFont)
    let lblGrade = new Label(Text = "Grade", Top = 210, Left = 70, ForeColor = Color.White, Font = commonFont)
    let txtGrade = new TextBox(Top = 210, Left = 180, Width = 250, Font = commonFont)

    let btnAddGrade = new Button(Text = "ADD", Top = 250, Left = 180, Width = 250, Height = 40, Font = commonFont, BackColor = Color.LightCoral)
    let btnupdGrade = new Button(Text = "Update", Top = 300, Left = 180, Width = 250, Height = 40, Font = commonFont, BackColor = Color.LightCoral)
    let btndltGrade = new Button(Text = "Delete", Top = 350, Left = 180, Width = 250, Height = 40, Font = commonFont, BackColor = Color.LightCoral)
    let btnExit = new Button(Text = "Exit", Top = 400, Left = 180, Width = 250, Height = 40, Font = commonFont, BackColor = Color.LightCoral)

    


    cmbCourse.AutoCompleteMode <- AutoCompleteMode.SuggestAppend 
    cmbCourse.AutoCompleteSource <- AutoCompleteSource.ListItems 
    cmbCourse.Validating.Add(fun _ ->
        if not (cmbCourse.Items.Contains(cmbCourse.Text)) then
            MessageBox.Show("Please select a valid course from the list.", "Invalid Course", MessageBoxButtons.OK, MessageBoxIcon.Warning)|> ignore
            cmbCourse.Text <- "")

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


    txtStudentID.AutoCompleteMode <- AutoCompleteMode.SuggestAppend
    txtStudentID.AutoCompleteSource <- AutoCompleteSource.CustomSource
    txtStudentID.AutoCompleteCustomSource <- autoCompleteCollection

    txtStudentID.KeyPress.Add(fun e ->
        if not (Char.IsDigit(e.KeyChar) || e.KeyChar = '\b') then  
            e.Handled <- true 
    )
    
    let query = "SELECT ID, course FROM courses"
    use command = new MySqlCommand(query, connection)
    use reader = command.ExecuteReader()

    let mutable courses = []
    while reader.Read() do
        let courseID = reader.GetInt32("ID")
        let courseName = reader.GetString("course")
        courses <- (courseID, courseName) :: courses
    reader.Close()


    courses |> List.iter (fun (_, courseName) -> cmbCourse.Items.Add(courseName) |> ignore)




    let caseStatements =
        courses
        |> List.map (fun (_, courseName) ->
            sprintf "MAX(CASE WHEN c.course = '%s' THEN sd.grades END) AS '%s'" courseName courseName)
        |> String.concat ", "

    let finalQuery = sprintf """
        SELECT s.ID, s.name, %s
        FROM students s
        LEFT JOIN students_details sd ON s.ID = sd.ID
        LEFT JOIN courses c ON sd.course_id = c.ID
        GROUP BY s.ID, s.name""" caseStatements

    let loadData () =
        use command1 = new MySqlCommand(finalQuery, connection)
        use reader1 = command1.ExecuteReader()
        let dataTable = new DataTable()
        dataTable.Load(reader1)
        dgvStudents.DataSource <- dataTable

    loadData ()

    btnAddGrade.Click.Add(fun _ ->
        let studentID = txtStudentID.Text
        let course = cmbCourse.SelectedItem.ToString()
        let grade = txtGrade.Text
        if not (String.IsNullOrEmpty(studentID) || String.IsNullOrEmpty(course) || String.IsNullOrEmpty(grade)) then
            addGrade studentID course grade
            loadData ()
    )

    btnupdGrade.Click.Add(fun _ ->
        let studentID = txtStudentID.Text
        let course = cmbCourse.SelectedItem.ToString()
        let grade = txtGrade.Text
        if not (String.IsNullOrEmpty(studentID) || String.IsNullOrEmpty(course) || String.IsNullOrEmpty(grade)) then
            updateGrade studentID course grade
            loadData ()
    )

    btndltGrade.Click.Add(fun _ ->
        let studentID = txtStudentID.Text
        let course = cmbCourse.SelectedItem.ToString()
        if not (String.IsNullOrEmpty(studentID) || String.IsNullOrEmpty(course)) then
            deleteGrade studentID course
            loadData ()
    )

    btnExit.Click.Add(fun _ ->
        form.Close()
        parentForm.Show()
    )

    form.Controls.AddRange([| lblStudentID; txtStudentID; lblCourse; cmbCourse; lblGrade; txtGrade; btnAddGrade; btnupdGrade; btndltGrade; btnExit; dgvStudents |])

    form.ShowDialog() |> ignore


let viewCourseStatisticsForm (parentForm: Form) =


    let form = new Form(Text = "Course Statistics", Width = 1500, Height = 1000, BackColor = Color.LightBlue)
    form.FormBorderStyle <- FormBorderStyle.Sizable  
    form.StartPosition <- FormStartPosition.CenterScreen
    let commonFont = new Font("Arial", 12.0f, FontStyle.Regular)



    let btnExit = new Button(Text = "Exit", Top = 900, Left = 725, Width = 150, Height = 40, BackColor = Color.LightCoral, Font = commonFont)

    let dgvStudents = new DataGridView(Top = 20, Left = 50, Width = 1400, Height = 850)
    dgvStudents.Font <- commonFont
    dgvStudents.AutoSizeColumnsMode <- DataGridViewAutoSizeColumnsMode.Fill
    dgvStudents.ColumnHeadersHeightSizeMode <- DataGridViewColumnHeadersHeightSizeMode.AutoSize
    dgvStudents.ReadOnly <- true


    let loadData () =
        let data = viewCourseStatistics()  
        dgvStudents.DataSource <- data

    loadData()  

    btnExit.Click.Add(fun _ -> 
        form.Close()
        parentForm.Show()
    )

    form.Controls.AddRange([| btnExit; dgvStudents |])
    form.ShowDialog() |> ignore


let instructorPanelForm (parentForm:Form) =
    let form = new Form(Text = "Instructor Panel", Width = 600, Height = 300, BackColor = Color.LightBlue)
    form.FormBorderStyle <- FormBorderStyle.Sizable  
    form.StartPosition <- FormStartPosition.CenterScreen

    let commonFont = new Font("Arial", 12.0f, FontStyle.Regular)

    let btnAddStudent = new Button(Text = "Grades",Top=50,Left=200, Width = 200, Height = 40, BackColor = Color.LightCoral, Font = commonFont)
    let btndtlStudent = new Button(Text = "Courses Details",Top=100,Left=200, Width = 200, Height = 40, BackColor = Color.LightCoral, Font = commonFont)
    let btnExit = new Button(Text = "Log Out",Top=150,Left=200, Width = 200, Height = 40, BackColor = Color.LightCoral, Font = commonFont)


    btnAddStudent.Click.Add(fun _ -> 
        form.Hide()
        addGradesForm form 
    )
    btndtlStudent.Click.Add(fun _ -> 
        form.Hide()
        viewCourseStatisticsForm form
    )
    btnExit.Click.Add(fun _ -> 
        form.Close()
        parentForm.Show()
    )

    form.Controls.AddRange([| btnAddStudent;btndtlStudent; btnExit |])

    form.ShowDialog() |> ignore

