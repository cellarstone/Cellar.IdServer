pipeline {
  agent any
  stages {
    
	stage('Build and Push IdentityServer4 to Myget') {
      steps {
        parallel(
          "IdentityServer4": {
            sh 'dotnet restore ./IdentityServer4'
            sh 'dotnet build ./IdentityServer4'
            sh 'dotnet pack ./IdentityServer4'
            sh 'dotnet nuget push ./IdentityServer4/bin/Debug/IdentityServer4cellarstone.2.0.0.nupkg -k c7afa313-d629-4dc7-84aa-2e658c2a3cce -s https://www.myget.org/F/cellar/api/v2/package'
          }
        )
      }
    }
    stage('Dotnet') {
      steps {
        parallel(
          "Cellar.IdServer": {
            sh 'dotnet restore ./Cellar.IdServer'
            sh 'dotnet build ./Cellar.IdServer'
            sh 'dotnet pack ./Cellar.IdServer'
            sh 'dotnet nuget push ./Cellar.IdServer/bin/Debug/Cellar.IdServer.0.0.1.nupkg -k c7afa313-d629-4dc7-84aa-2e658c2a3cce -s https://www.myget.org/F/cellar/api/v2/package'
            sh 'dotnet publish'
          }
        )
      }
    }
    stage('Docker') {
      steps {
        parallel(
          "Cellar.IdServer": {
            sh 'docker build -t cellar.idserver ./Cellar.IdServer'
            sh 'docker tag cellar.idserver eu.gcr.io/cellarstone-1488228226623/cellar.idserver:0.0.1'
            sh 'gcloud docker -- push eu.gcr.io/cellarstone-1488228226623/cellar.idserver:0.0.1'
            
          }
        )
      }
    }
	stage('Deploy to Staging') {
      steps {
        parallel(
		"Sql": {
			sh 'echo "Deploy to Staging done!"'

          },
		"Mqtt": {
			sh 'echo "Deploy to Staging done!"'

          },
		"Web": {
			sh 'echo "Deploy to Staging done!"'

          }
        )
      }
    }
	stage('Staging human check') {
            steps {
                input "Does the staging environment look ok?"
            }
    }
    stage('Deploy to Production') {
      steps {
        parallel(
		"Sql": {
			sh 'echo "Deploy to Production done!"'

          },
		"Mqtt": {
			sh 'echo "Deploy to Production done!"'

          },
		"Web": {
			sh 'echo "Deploy to Production done!"'

          }
        )
      }
    }
  }
}