pipeline {
  agent any
  stages {
    
	stage('Build IdentityServer4') {
      steps {
        
            sh 'dotnet restore ./IdentityServer4'
            sh 'dotnet build ./IdentityServer4'
            // sh 'dotnet pack ./IdentityServer4'
            // sh 'dotnet nuget push ./IdentityServer4/bin/Debug/IdentityServer4cellarstone.0.0.2.nupkg -k c7afa313-d629-4dc7-84aa-2e658c2a3cce -s https://www.myget.org/F/cellar/api/v2/package'
          
      }
    }
    stage('Development') {
      steps {
            sh 'dotnet restore ./Cellar.IdServer --configfile NuGet.Config'
            sh 'export ASPNETCORE_ENVIRONMENT=Development'
            sh 'dotnet build ./Cellar.IdServer'
            sh 'dotnet publish ./Cellar.IdServer'
            sh 'docker build -t cellar.idserver ./Cellar.IdServer'
            sh 'docker tag cellar.idserver eu.gcr.io/cellarstone-1488228226623/cellar.idserver:dev.0.0.1'
            sh 'gcloud docker -- push eu.gcr.io/cellarstone-1488228226623/cellar.idserver:dev.0.0.1'
            sh 'gcloud docker -- push eu.gcr.io/cellarstone-1488228226623/cellar.idserver:dev.0.0.1'
      }
    }
    stage('Human Check - deploy to Staging') {
            steps {
                input "Does the development environment look ok?"
            }
    }
    stage('Staging') {
      steps {
        
            sh 'export ASPNETCORE_ENVIRONMENT=Staging'
            sh 'dotnet build ./Cellar.IdServer --configuration Release'
            sh 'dotnet publish ./Cellar.IdServer'
            sh 'docker build -t cellar.idserver ./Cellar.IdServer'
            sh 'docker tag cellar.idserver eu.gcr.io/cellarstone-1488228226623/cellar.idserver:stag.0.0.1'
            sh 'gcloud docker -- push eu.gcr.io/cellarstone-1488228226623/cellar.idserver:stag.0.0.1'
         
        
      }
    }
    stage('Human Check - deploy to Production') {
            steps {
                input "Does the Staging environment look ok?"
            }
    }
    stage('Production') {
      steps {
        
            sh 'export ASPNETCORE_ENVIRONMENT=Production'
            sh 'dotnet build ./Cellar.IdServer --configuration Release'
            sh 'dotnet publish ./Cellar.IdServer'
            sh 'docker build -t cellar.idserver ./Cellar.IdServer'
            sh 'docker tag cellar.idserver eu.gcr.io/cellarstone-1488228226623/cellar.idserver:prod.0.0.1'
            sh 'gcloud docker -- push eu.gcr.io/cellarstone-1488228226623/cellar.idserver:prod.0.0.1'
          
      }
    }
  }
}