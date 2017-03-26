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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace github_todoist_net
{
    /// <summary>
    /// Interaction logic for SelectableIssue.xaml
    /// </summary>
    public partial class SelectableIssue : UserControl
    {
        public Octokit.Issue Issue { get; private set; } = null;

        public SelectableIssue(Octokit.Issue _issue)
        {
            InitializeComponent();
            Issue = _issue;

            if (Issue == null)
            {
                throw new ArgumentNullException("_issue");
            }

            IssueNumber.Text = Issue.Number.ToString();
            IssueTitle.Text = Issue.Title;
        }

        public bool Checked
        {
            get
            {
                return Selected.IsChecked ?? false;
            }
        }
    }
}
