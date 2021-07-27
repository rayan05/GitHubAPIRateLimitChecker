pipeline {
  agent { dockerfile true }
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
                def   dockerImage = docker.build("temptestcicd.azurecr.io/ratelimitchecker:latest")
                dockerImage.push 'latest'
            }
        }
      }
    }
  }
}
