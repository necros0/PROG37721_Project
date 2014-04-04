﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace InventoryManagement
{
    public partial class Form2 : Form
    {
        bool needsUpdate = false;
        DataRow data;
        String insertOrUpdate;

        Regex availabilityRegex = new Regex(@"^\d*$"),
              costRegex = new Regex(@"^\$?\d*\.?\d{0,2}$");

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(DataRow row, string state)
        {
            InitializeComponent();
            data = row;
            insertOrUpdate = state;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            populateComboBoxes();
            txtName.Text = data["name"].ToString();
            txtAvailability.Text = data["availability"].ToString();
            if (data["cost"] != DBNull.Value)
            {
                txtCost.Text = Convert.ToDouble(data["cost"]).ToString("c");
            }
            cboPlatform.Text = data["platform"].ToString();
            cboPublisher.Text = data["publisher"].ToString();
            cboDeveloper.Text = data["developer"].ToString();
            cboCategory.Text = data["category"].ToString();

            if (data["releaseDate"] != DBNull.Value)
            {
                dtpReleaseDate.Value = (DateTime)data["releaseDate"];
            }

            //List Rating Combobox
            listRatings();
            cboRating.Text = data["rating"].ToString();

            txtAvailability.TextChanged += new EventHandler(checkRegexFields);
            txtCost.TextChanged += new EventHandler(checkRegexFields);
        }

        void checkRegexFields(object sender, EventArgs e)
        {
            cmdSave.Enabled = true;

            if (!availabilityRegex.IsMatch(txtAvailability.Text))
            {
                // show error label
                cmdSave.Enabled = false;
            }
            if (!costRegex.IsMatch(txtCost.Text))
            {
                // show error label
                cmdSave.Enabled = false;
            }
        }

        /**
         * Populate combo boxes with all distinct values from the database for the
         * appropriate combo boxes.
         * Uses Form1's static method and dataset contained in Form1 static class.
         */
        private void populateComboBoxes()
        {
            Form1.populateComboBox(cboCategory, Form1.COL_CATEGORY);
            Form1.populateComboBox(cboDeveloper, Form1.COL_DEVELOPER);
            Form1.populateComboBox(cboPlatform, Form1.COL_PLATFORM);
            Form1.populateComboBox(cboPublisher, Form1.COL_PUBLISHER);
            //Form1.populateComboBox(cboRating, Form1.COL_RATING);
            //CJ: Not sure why but Add New Item would not populate with the right values (the original line here was wrong anyway)
            cboRating.DropDownStyle = ComboBoxStyle.DropDownList; //CJ: Adding this; was it supposed to allow you to type in here?  this is a domain that is unlikely to change.
            string[] ratings = { "E", "EC", "E10+", "T", "M", "AO", "RP" };
            cboRating.Items.AddRange(ratings); //CJ: Add all items at once.
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            //Insert or update record and check for empty field
            String connStr = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True;User Instance=True";
            SqlConnection conn = new SqlConnection(connStr);
            String sql = "SELECT * FROM [inventory]";
            SqlDataAdapter dAdapter = new SqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            dAdapter.Fill(dt);
            if (isEmptyField() != true && isValidPrimaryKey(dt, txtName.Text, cboPlatform.Text) == true && insertOrUpdate.Equals("insert"))
            {
                data["name"] = txtName.Text;
                data["availability"] = Convert.ToInt32(txtAvailability.Text);
                String cost = txtCost.Text;
                if (cost[0] == '$')
                {
                    cost = cost.Substring(1);
                }
                data["cost"] = Convert.ToDouble(cost);

                data["platform"] = cboPlatform.Text;
                data["publisher"] = cboPublisher.Text;
                data["developer"] = cboDeveloper.Text;
                data["rating"] = cboRating.Text;
                data["category"] = cboCategory.Text;

                data["releaseDate"] = dtpReleaseDate.Value.Date;

                needsUpdate = true;
                Dispose();
            }
            else if (isEmptyField() != true && insertOrUpdate.Equals("update"))
            {
                data["name"] = txtName.Text;
                data["availability"] = Convert.ToInt32(txtAvailability.Text);
                String cost = txtCost.Text;
                if (cost[0] == '$')
                {
                    cost = cost.Substring(1);
                }
                data["cost"] = Convert.ToDouble(cost);

                data["platform"] = cboPlatform.Text;
                data["publisher"] = cboPublisher.Text;
                data["developer"] = cboDeveloper.Text;
                data["rating"] = cboRating.Text;
                data["category"] = cboCategory.Text;

                data["releaseDate"] = dtpReleaseDate.Value.Date;

                needsUpdate = true;
                Dispose();
            }
        }

        new public bool ShowDialog()
        {
            base.ShowDialog();
            return needsUpdate;
        }

        private void listRatings()
        {
            cboRating.Items.Clear();
            cboRating.DropDownStyle = ComboBoxStyle.DropDownList;
            string[] ratings = { "E", "EC", "E10+", "T", "M", "AO", "RP" };
            cboRating.Items.AddRange(ratings); //CJ: Add all items at once.
        }

        private bool isEmptyField()
        {
            if (txtName.Text.Equals(""))
            {
                MessageBox.Show("Name Field Empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return true;
            }
            else if (cboPlatform.Text.Equals(""))
            {
                MessageBox.Show("Platform Field Empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboPlatform.Focus();
                return true;
            }
            else if (cboPublisher.Text.Equals(""))
            {
                MessageBox.Show("Publisher Field Empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboPublisher.Focus();
                return true;
            }
            else if (cboDeveloper.Text.Equals(""))
            {
                MessageBox.Show("Developer Field Empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboDeveloper.Focus();
                return true;
            }
            else if (cboRating.Text.Equals(""))
            {
                MessageBox.Show("Rating Field Empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboRating.Focus();
                return true;
            }
            else if (cboCategory.Text.Equals(""))
            {
                MessageBox.Show("Category Field Empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboCategory.Focus();
                return true;
            }
            else if (txtAvailability.Text.Equals(""))
            {
                MessageBox.Show("Availability Field Empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAvailability.Focus();
                return true;
            }
            else if (new Regex(@"^\$?\.?$").IsMatch(txtCost.Text))
            {
                MessageBox.Show("Cost Field Empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCost.Focus();
                return true;
            }
            return false;
        }
        private bool isValidPrimaryKey(DataTable dt, string name, string platform)
        {
            object[] pk = new object[2];
            pk[0] = name;
            pk[1] = platform;
            dt.PrimaryKey = new DataColumn[] { dt.Columns["name"], dt.Columns["platform"] };
            DataRow foundRow = dt.Rows.Find(pk);
            if (foundRow != null && insertOrUpdate.Equals("insert"))
            {
                MessageBox.Show("Game Name in Platform Exists!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }
            return true;
        }
    }
}
