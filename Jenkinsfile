pipeline {
  agent any
  environment {
    registryName = 'TempTestCICD'
    registryCredential  = 'ACR'
    dockerImage = ''
    registryUrl = 'temptestcicd.azurecr.io'
  }
  stages {
    stage('publish docker') {
      steps{
        script{
            docker.withRegistry("http://${registryUrl}", registryCredential) {
                def   dockerImage = docker.build("temptestcicd.azurecr.io/APIRateLimitChecker:latest")
                dockerImage.push 'latest'
            }
        }
      }
    }
  }
}
