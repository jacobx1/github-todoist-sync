# GitHub to Todoist Issue Importer

[![Build status](https://ci.appveyor.com/api/projects/status/yn4gej2oh84jsjeo?svg=true)](https://ci.appveyor.com/project/jacobx1/github-todoist-sync)

Import open issues from GitHub milestones to a Todoist Project.

## Setup

1. Generate a GitHub personal access token - https://help.github.com/articles/creating-a-personal-access-token-for-the-command-line/
	+ Need access to read repo data - issues and milestones
2. Get your todoist api token
	+ Todoist > Settings > Account
3. _(Optional)_ Change GitHub url for Enterprise installations

Settings are saved after each refresh.

## Usage

After setting up the importer and doing a refresh to pull data:

1. Select repository to get issues from
2. Select milestone for import
3. Select Todoist project to import issues to
4. Click `===>` to import from milestone to Todoist Project
