# GitHubAPIRateLimitTracker

This application checks GitHub's API rate limit is in safe limit. For the given PAT, the remaining API rate is more less than 10% this application return 1 Else it will return 0
If any error occured this will return -1


This abblication will build & run on Linux based docker containers

## How to run this apllication

1. Download or Clone the Apllication
2. CD GitHubAPIRateLimitChecker
3. docker build -t apiratechecker-image -f DockerFile .
4. docker run -it --rm apiratechecker-image
5. When it ask enter the GitHub Persona Access Token (PAT)


## prerequisite
Need to have docker desktop installed in your computer.

