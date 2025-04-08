Imports System.Data.SqlClient

Public Class BudgetApp
    Dim sqlCon As New SqlConnection("Data Source=DESKTOP-LTHP06O;Initial Catalog=Semester_Project;Integrated Security=True;Encrypt=False")

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Application.Exit()
    End Sub

    Private Sub LoadMonths()
        'CboMonths.Items.Clear()
        Dim months As String() = {"January", "February", "March", "April", "May", "June",
                              "July", "August", "September", "October", "November", "December"}

        CboMonths.Items.AddRange(months)
        CboMonths.SelectedIndex = DateTime.Now.Month - 1
    End Sub
    Private Sub LoadBudgetMonths()
        ComboBox4.Items.Clear()
        Dim months As String() = {"January", "February", "March", "April", "May", "June",
                              "July", "August", "September", "October", "November", "December"}

        ComboBox4.Items.AddRange(months)
        ComboBox4.SelectedIndex = DateTime.Now.Month - 1
    End Sub

    Private Sub LoadExpenseMonths()
        ComboBox5.Items.Clear()
        Dim months As String() = {"January", "February", "March", "April", "May", "June",
                              "July", "August", "September", "October", "November", "December"}

        ComboBox5.Items.AddRange(months)
        ComboBox5.SelectedIndex = DateTime.Now.Month - 1
    End Sub
    Private Sub LoadExpenseCat()
        ComboBox2.Items.Clear()
        Dim months As String() = {"Food", "Groceries", "T&T", "utilities"}

        ComboBox2.Items.AddRange(months)
        ComboBox2.SelectedIndex = DateTime.Now.Month - 1
    End Sub
    Private Sub LoadFilterExpenseCat()
        ComboBox1.Items.Clear()
        Dim months As String() = {"Food", "Groceries", "T&T", "utilities"}

        ComboBox1.Items.AddRange(months)
        ComboBox1.SelectedIndex = DateTime.Now.Month - 1
    End Sub

    Private Sub LoadFilterExpenseMonths()
        ComboBox3.Items.Clear()
        Dim months As String() = {"January", "February", "March", "April", "May", "June",
                              "July", "August", "September", "October", "November", "December"}

        ComboBox3.Items.AddRange(months)
        ComboBox3.SelectedIndex = DateTime.Now.Month - 1
    End Sub
    Private Sub LoadExpenses()
        Try
            sqlCon.Open()
            Dim sqlquery As String = "SELECT * FROM Expenses"
            Dim sqlCmd As New SqlCommand(sqlquery, sqlCon)
            Dim adapter As New SqlDataAdapter(sqlCmd)
            Dim dt As New DataTable()
            adapter.Fill(dt)
            DataGridView1.DataSource = dt
            sqlCon.Close()

            DataGridView1.AutoGenerateColumns = False

            AddActionButtons()

        Catch ex As Exception
            MessageBox.Show("Error loading expenses: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            sqlCon.Close()
        End Try
    End Sub

    Private Sub LoadBudget()
        If CboMonths.SelectedIndex = -1 Then Exit Sub

        Dim selectedMonth As String = CboMonths.SelectedItem.ToString()
        'Dim currentYear As Integer = DateTime.Now.Year
        Try
            sqlCon.Open()
            Dim query As String = "SELECT BudgetAmount FROM Budgets WHERE Month = @Month"
            Dim sqlCmd As New SqlCommand(query, sqlCon)
            sqlCmd.Parameters.AddWithValue("@Month", selectedMonth)
            'sqlCmd.Parameters.AddWithValue("@Year", DateTime.Now.Year)

            Dim result As Object = sqlCmd.ExecuteScalar()

            If result IsNot Nothing Then

                LblBudgetResult.Text = result.ToString()
            Else
                MessageBox.Show("There no Budget for the selected Month!", "Worning", MessageBoxButtons.OK, MessageBoxIcon.Warning)

                LblBudgetResult.Text = "No Budget"
            End If
            sqlCon.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading budget: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            sqlCon.Close()
        End Try
    End Sub

    Private Function CalculateTotalExpenses() As Decimal
        Dim totalExpenses As Decimal = 0

        Try
            sqlCon.Open()
            Dim query As String = "SELECT SUM(ItemAmount) FROM Expenses"
            Dim sqlcmd As New SqlCommand(query, sqlCon)
            Dim result = sqlcmd.ExecuteScalar()

            If result IsNot DBNull.Value Then
                totalExpenses = Convert.ToDecimal(result)

            End If
        Catch ex As Exception
            MessageBox.Show("Error calculating total expenses: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If sqlCon.State = ConnectionState.Open Then
                sqlCon.Close()
            End If
        End Try

        ' Display total expenses
        LblExpenseResult.Text = totalExpenses.ToString("N2")

        Return totalExpenses
    End Function

    Private Sub CalculateBalance()
        Dim totalBudget As Decimal
        Dim totalExpenses As Decimal = CalculateTotalExpenses()

        ' Ensure LblBudgetResult.Text contains a valid number
        If Decimal.TryParse(LblBudgetResult.Text, totalBudget) Then
            ' If conversion succeeds, calculate balance
            Dim balance As Decimal = totalBudget - totalExpenses
            LblBalanceResult.Text = balance.ToString("N2")
        Else
            ' If there is no budget, set balance to negative total expenses
            LblBalanceResult.Text = (-totalExpenses).ToString("N2")
        End If
    End Sub

    Private Sub BudgetApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadMonths()
        LoadBudgetMonths()
        LoadExpenseMonths()
        LoadExpenseCat()
        LoadFilterExpenseCat()
        LoadFilterExpenseMonths()
        LoadExpenses()
        LoadBudget()
        CalculateTotalExpenses()
        CalculateBalance()
    End Sub

    Private Sub AddActionButtons()
        ' Clear existing columns to prevent duplication
        Dim btnEdit As New DataGridViewButtonColumn()
        btnEdit.HeaderText = "Edit"
        btnEdit.Name = "Edit"
        btnEdit.Text = "Edit"
        btnEdit.UseColumnTextForButtonValue = True
        DataGridView1.Columns.Add(btnEdit)

        ' Update Button
        Dim btnUpdate As New DataGridViewButtonColumn()
        btnUpdate.HeaderText = "Update"
        btnUpdate.Name = "Update"
        btnUpdate.Text = "Update"
        btnUpdate.UseColumnTextForButtonValue = True
        DataGridView1.Columns.Add(btnUpdate)

        ' Delete Button
        Dim btnDelete As New DataGridViewButtonColumn()
        btnDelete.HeaderText = "Delete"
        btnDelete.Name = "Delete"
        btnDelete.Text = "Delete"
        btnDelete.UseColumnTextForButtonValue = True
        DataGridView1.Columns.Add(btnDelete)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'Dim expenseId As Integer = TextBox3.Text
        Dim expenseyear As Integer = CInt(TextBox8.Text.Trim())
        Dim expenseMonth As String = ComboBox5.Text.Trim()
        Dim expenseName As String = TextBox7.Text.Trim()
        Dim expenseCat As String = ComboBox2.Text.Trim()
        Dim expenseAmount As Decimal = CDec(TextBox1.Text.ToString())
        Dim expenseDate As Date = DateTimePicker2.Value.ToString("yyyy-MM-dd")

        Try
            sqlCon.Open()

            Dim sqlquery As String = "INSERT INTO Expenses (Year,Month, ItemName, Category, ItemAmount, ExpenseDate) VALUES ( @Year, @Month, @ItemName, @Category, @ItemAmount, @ExpenseDate)"
            Dim sqlCmd As New SqlCommand(sqlquery, sqlCon)

            'sqlCmd.Parameters.AddWithValue("@ID", expenseId)
            sqlCmd.Parameters.AddWithValue("@Year", expenseyear)
            sqlCmd.Parameters.AddWithValue("@Month", expenseMonth)
            sqlCmd.Parameters.AddWithValue("@ItemName", expenseName)
            sqlCmd.Parameters.AddWithValue("@Category", expenseCat)
            sqlCmd.Parameters.AddWithValue("@ItemAmount", expenseAmount)
            sqlCmd.Parameters.AddWithValue("@ExpenseDate", expenseDate)

            sqlCmd.ExecuteNonQuery()
            sqlCon.Close()
            LoadExpenses()
            CalculateTotalExpenses()
            CalculateBalance()
            MessageBox.Show("Expense added successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            TextBox8.Text = " "
            ComboBox5.Text = " "
            TextBox7.Text = " "
            ComboBox2.Text = " "
            TextBox1.Text = " "

        Catch ex As Exception
            MessageBox.Show("Error adding expense: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally

            sqlCon.Close()
        End Try

    End Sub

    Private Sub FilterExpensesByMonth(selectedMonth As String)
        Try
            sqlCon.Open()
            Dim sqlQuery As String = "SELECT * FROM Expenses WHERE Month = @Month"
            Dim sqlCmd As New SqlCommand(sqlQuery, sqlCon)
            sqlCmd.Parameters.AddWithValue("@Month", selectedMonth)

            Dim adapter As New SqlDataAdapter(sqlCmd)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            DataGridView1.DataSource = dt
            sqlCon.Close()
            If dt.Rows.Count = 0 Then
                LblExpenseResult.Text = "0.00"
            Else
                CalculateTotalExpenses()

            End If
            CalculateBalance()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            sqlCon.Close()
        End Try
    End Sub

    Private Sub CboMonths_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboMonths.SelectedIndexChanged
        LoadBudget()
        CalculateBalance()
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.RowIndex >= 0 Then
            Dim selectedID As Integer = Convert.ToInt32(DataGridView1.Rows(e.RowIndex).Cells(0).Value)

            'Dim ExpenseId As String = DataGridView1.CurrentRow.Cells(0).Value.ToString()

            If DataGridView1.Columns(e.ColumnIndex).Name = "Edit" Then
                TextBox8.Text = DataGridView1.CurrentRow.Cells(1).Value
                ComboBox5.Text = DataGridView1.CurrentRow.Cells(2).Value
                TextBox7.Text = DataGridView1.CurrentRow.Cells(3).Value
                ComboBox2.Text = DataGridView1.CurrentRow.Cells(4).Value
                TextBox1.Text = DataGridView1.CurrentRow.Cells(5).Value
                Button2.Enabled = False

            ElseIf DataGridView1.Columns(e.ColumnIndex).Name = "Update" Then

                Dim expenseyear As Integer = CInt(TextBox8.Text.Trim())
                Dim expenseMonth As String = ComboBox5.Text.Trim()
                Dim expenseName As String = TextBox7.Text.Trim()
                Dim expenseCat As String = ComboBox2.Text.Trim()
                Dim expenseAmount As Decimal = CDec(TextBox1.Text.ToString())
                Dim expenseDate As Date = DateTimePicker2.Value.ToString("yyyy-MM-dd")


                Try
                    sqlCon.Open()
                    Dim sqlQuery As String = "UPDATE Expenses SET Year = @Year, Month =@Month, ItemName = @ItemName, Category = @Category, ItemAmount = @ItemAmount, ExpenseDate = @ExpenseDate WHERE ID = @ID"

                    Dim sqlCmd As New SqlCommand(sqlQuery, sqlCon)
                    sqlCmd.Parameters.AddWithValue("@ID", selectedID)
                    sqlCmd.Parameters.AddWithValue("@Year", expenseyear)
                    sqlCmd.Parameters.AddWithValue("@Month", expenseMonth)
                    sqlCmd.Parameters.AddWithValue("@ItemName", expenseName)
                    sqlCmd.Parameters.AddWithValue("@Category", expenseCat)
                    sqlCmd.Parameters.AddWithValue("@ItemAmount", expenseAmount)
                    sqlCmd.Parameters.AddWithValue("@ExpenseDate", expenseDate)

                    sqlCmd.ExecuteNonQuery()
                    sqlCon.Close()
                    LoadExpenses()
                    CalculateTotalExpenses()
                    CalculateBalance()

                    MessageBox.Show("Expense Updated successfully", "information", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                    TextBox8.Text = " "
                    ComboBox5.Text = " "
                    TextBox7.Text = " "
                    ComboBox2.Text = " "
                    TextBox1.Text = " "

                Catch ex As Exception
                    MessageBox.Show("Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    sqlCon.Close()
                End Try

                'UpdateExpense(selectedID)
            ElseIf DataGridView1.Columns(e.ColumnIndex).Name = "Delete" Then
                'Dim ExpenseId As String = DataGridView1.CurrentRow.Cells(0).Value.ToString()
                Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

                If result = DialogResult.Yes Then
                    Try
                        sqlCon.Open()
                        Dim sqlQuery As String = "DELETE FROM Expenses WHERE ID = @ID"
                        Dim sqlCmd As New SqlCommand(sqlQuery, sqlCon)

                        ' Bind parameter to prevent SQL injection
                        sqlCmd.Parameters.AddWithValue("@ID", selectedID)

                        sqlCmd.ExecuteNonQuery()
                        sqlCon.Close()
                        LoadExpenses()
                        CalculateTotalExpenses()
                        CalculateBalance()

                        MessageBox.Show("expense deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        MessageBox.Show("Database Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Finally
                        sqlCon.Close()
                    End Try
                End If


                'DeleteExpense(selectedID)
            End If
        End If
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged
        Dim selectedMonth As String = ComboBox3.Text.Trim()
        FilterExpensesByMonth(selectedMonth)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'Dim expenseId As Integer = TextBox3.Text
        Dim budgetyear As Integer = CInt(TextBox6.Text.Trim())
        Dim budgetMonth As String = ComboBox4.Text.Trim()
        Dim budgetAmount As Decimal = CDec(TextBox5.Text.ToString())

        Try
            sqlCon.Open()

            Dim sqlquery As String = "INSERT INTO Budgets (Year,Month, BudgetAmount) VALUES ( @Year, @Month, @BudgetAmount)"
            Dim sqlCmd As New SqlCommand(sqlquery, sqlCon)

            'sqlCmd.Parameters.AddWithValue("@ID", expenseId)
            sqlCmd.Parameters.AddWithValue("@Year", budgetyear)
            sqlCmd.Parameters.AddWithValue("@Month", budgetMonth)
            sqlCmd.Parameters.AddWithValue("@BudgetAmount", budgetAmount)


            sqlCmd.ExecuteNonQuery()
            sqlCon.Close()
            LoadBudget()
            CalculateTotalExpenses()
            CalculateBalance()
            MessageBox.Show("budget added successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            TextBox8.Text = " "
            ComboBox5.Text = " "
            TextBox7.Text = " "
            ComboBox2.Text = " "
            TextBox1.Text = " "

        Catch ex As Exception
            MessageBox.Show("Error adding expense: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally

            sqlCon.Close()
        End Try
    End Sub


End Class