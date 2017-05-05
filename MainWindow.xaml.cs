using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace github_todoist_net
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Octokit.GitHubClient m_client = null;
        Todoist.Net.TodoistClient m_todoistClient = null;
        Octokit.User m_currentUser = null;

        class RepositoryItem
        {
            public long id = 0;
            public string name;
            public override string ToString()
            {
                return name;
            }
        }

        class MilestoneItem
        {
            public int number = 0;
            public string name;
            public override string ToString()
            {
                return name;
            }
        }

        class TodoistItem
        {
            public Todoist.Net.Models.ComplexId id;
            public string name;

            public override string ToString()
            {
                return name;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            GithubToken.Password = Properties.Settings.Default.GithubToken;
            TodoistToken.Password = Properties.Settings.Default.TodoistToken;
            GithubURL.Text = Properties.Settings.Default.GithubURL;
        }

        async void Connect(string githubUrl, string githubToken, string todoistToken)
        {
            m_client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("github-todoist"), new Uri(githubUrl));
            m_client.Credentials = new Octokit.Credentials(githubToken);

            m_currentUser = await m_client.User.Current();

            GithubUserName.Content = String.Format("Connected: {0}", m_currentUser.Login);
            GithubUserName.Foreground = Brushes.Green;

            var allRepos = await m_client.Repository.GetAllForCurrent();

            Repository.Items.Clear();
            foreach (var repo in allRepos)
            {
                var repoItem = new RepositoryItem() { name = repo.FullName, id = repo.Id };
                Repository.Items.Add(repoItem);
            }

            m_todoistClient = new Todoist.Net.TodoistClient(todoistToken);

            var projects = await m_todoistClient.Projects.GetAsync();

            TodoistProjects.Items.Clear();
            foreach (var project in projects)
            {
                var todoistItem = new TodoistItem() { name = project.Name, id = project.Id };
                TodoistProjects.Items.Add(todoistItem);
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.GithubToken = GithubToken.Password;
            Properties.Settings.Default.TodoistToken = TodoistToken.Password;
            Properties.Settings.Default.GithubURL = GithubURL.Text;

            Properties.Settings.Default.Save();

            Connect(GithubURL.Text, GithubToken.Password, TodoistToken.Password);
        }

        private async void Sync_Click(object sender, RoutedEventArgs e)
        {
            var milestoneItem = GithubMilestones.SelectedItem as MilestoneItem;
            var projectItem = TodoistProjects.SelectedItem as TodoistItem;
            var repoItem = Repository.SelectedItem as RepositoryItem;
            
            if (projectItem == null || repoItem == null)
            {
                return;
            }

            var issueRequest = new Octokit.RepositoryIssueRequest() { Assignee = m_currentUser.Login, State = Octokit.ItemStateFilter.Open };
            if (milestoneItem != null && milestoneItem.number != 0)
            {
                issueRequest.Milestone = milestoneItem.number.ToString();
            }

            var issues = await m_client.Issue.GetAllForRepository(repoItem.id, issueRequest);

            if (issues == null)
            {
                return;
            }

            var selectIssuesForm = new IssueSelect(issues);
            if (!(selectIssuesForm.ShowDialog() ?? false))
            {
                return;
            }

            var selectedIssues = selectIssuesForm.SelectedIssues;

            SyncProgress.Maximum = selectedIssues.Count;
            SyncProgress.Value = 0;

            foreach (var issue in selectedIssues)
            {
                var title = String.Format("**{0}** [{1}]({2})", issue.Number, issue.Title, issue.HtmlUrl.ToString());

                var todoistItem = new Todoist.Net.Models.Item(title, projectItem.id);
                await m_todoistClient.Items.AddAsync(todoistItem);

                ++SyncProgress.Value;
            }
        }

        private async void Repository_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GithubMilestones.Items.Clear();

            if (e.AddedItems.Count == 0)
            {
                return;
            }

            var item = e.AddedItems[0] as RepositoryItem;
            if (item == null)
            {
                return;
            }

            GithubMilestones.Items.Add(new MilestoneItem() { name = "None", number = 0 });
            GithubMilestones.SelectedIndex = 0;

            var milestones = await m_client.Issue.Milestone.GetAllForRepository(item.id);
            foreach (var milestone in milestones)
            {
                var milestoneItem = new MilestoneItem() { name = milestone.Title, number = milestone.Number };
                GithubMilestones.Items.Add(milestoneItem);
            }
        }
    }
}
