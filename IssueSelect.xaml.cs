using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace github_todoist_net
{
    /// <summary>
    /// Interaction logic for IssueSelect.xaml
    /// </summary>
    public partial class IssueSelect : Window
    {
        public IssueSelect(IReadOnlyList<Octokit.Issue> issues)
        {
            InitializeComponent();

            if (issues == null)
            {
                throw new ArgumentNullException("issues");
            }

            foreach (var issue in issues)
            {
                var itemBox = new SelectableIssue(issue) { Checked = true };
                IssuesList.Items.Add(itemBox);
            }
        }

        public List<Octokit.Issue> SelectedIssues
        {
            get
            {
                var ret = new List<Octokit.Issue>();

                foreach (var item in IssuesList.Items)
                {
                    var itemBox = item as SelectableIssue;
                    if (itemBox == null)
                    {
                        continue;
                    }

                    if (itemBox.Checked)
                    {
                        ret.Add(itemBox.Issue);
                    }
                }

                return ret;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
