open System
open System.Drawing
open System.Windows.Forms
open MySql.Data.MySqlClient


let connectionString = "Server=localhost;Port=3306;Database=sms;User ID=root;Password=;"

type Role =
    | SuperUser 
    | Instructor
    | Student of int32
    
//Login Functions Section
let getRoleFromType (userType: string) (studentID:int32 ) =
    match userType.ToLower() with
    | "superuser" -> Some SuperUser
    | "instructor" -> Some Instructor
    | "student" -> Some (Student studentID)
    | _ -> None

let login username password =
    use connection = new MySqlConnection(connectionString)
    connection.Open()

    let query = "SELECT user_type, password, ID FROM users WHERE username = @username"
    use command = new MySqlCommand(query, connection)
    command.Parameters.AddWithValue("@username", username) |> ignore

    use reader = command.ExecuteReader()

    if reader.Read() && reader.GetString(1) = password then
        let userType = reader.GetString(0) 
        let studentID = reader.GetInt32(2)
        
        getRoleFromType userType studentID
    else
        None


let loginForm () =
    let form = new Form(Text = "Login", Width = 400, Height = 250, StartPosition = FormStartPosition.CenterScreen, BackColor = Color.LightBlue)

    let lblUsername = new Label(Text = "Username", Top = 50, Left = 10, ForeColor = Color.White)
    let txtUsername = new TextBox(Top = 50, Left = 120, Width = 200, BackColor = Color.White, ForeColor = Color.Black)

    let lblPassword = new Label(Text = "Password", Top = 110, Left = 10, ForeColor = Color.White)
    let txtPassword = new TextBox(Top = 110, Left = 120, Width = 200, BackColor = Color.White, ForeColor = Color.Black)
    txtPassword.UseSystemPasswordChar <- true

    let btnLogin = new Button(Text = "Login", Top = 150, Left = 100, Width = 200, Height = 40, BackColor = Color.LightCoral, ForeColor = Color.White, Font = new Font("Arial", 12.0f))


    btnLogin.Click.Add(fun _ -> 
        let username = txtUsername.Text
        let password = txtPassword.Text
        if not (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password)) then
            match login username password with
            | Some SuperUser ->

                form.Hide()
                superUserPanelForm ()
            | Some Instructor -> 

                form.Hide()
                instructorPanelForm ()
            | Some (Student studentID) -> 

                form.Hide()
                studentPanelForm studentID  
            | _ -> 
                MessageBox.Show("Invalid username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        else
            MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
    )

    form.Controls.AddRange([| lblUsername; txtUsername; lblPassword; txtPassword; btnLogin |])
    form.ShowDialog() |> ignore
//Main section
[<STAThread>]
do
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault(false)
    loginForm ()
