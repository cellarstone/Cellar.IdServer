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

            sh 'gcloud container clusters get-credentials developcluster-1 --zone europe-west1-b --project cellarstone-1488228226623'
            sh 'kubectl apply -f k8s/dev/secrets.yaml'
            sh 'kubectl apply -f k8s/dev/deployments.yaml'
            sh 'kubectl apply -f k8s/dev/services.yaml'
      }
    }
    stage('Human Check') {
            steps {
                input "Can I deploy to Staging ?"
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
         
            sh 'gcloud container clusters get-credentials stagingcluster-1 --zone europe-west2-a --project cellarstone-1488228226623'
            sh 'kubectl apply -f k8s/stag/secrets.yaml'
            sh 'kubectl apply -f k8s/stag/deployments.yaml'
            sh 'kubectl apply -f k8s/stag/services.yaml'
      }
    }
    stage('Human Check') {
            steps {
                input "Can I deploy to Production ?"
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
          
            sh 'gcloud container clusters get-credentials productioncluster-1 --zone us-west1-a --project cellarstone-1488228226623'
            sh 'kubectl apply -f k8s/prod/secrets.yaml'
            sh 'kubectl apply -f k8s/prod/deployments.yaml'
            sh 'kubectl apply -f k8s/prod/services.yaml'
      }
    }
  }
}