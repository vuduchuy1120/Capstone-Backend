pipeline {
    agent any

    tools {
        dotnetsdk "DotNet8"
    }
    
    stages {

        stage('Run unit test') {
            steps {
                script {
                    sh 'dotnet test'
                }
            }
        }

        stage('Run docker compose build') {
            steps {
                script {
                    sh 'docker compose build'
                }
            }
        }

        stage('Run docker compose') {
            steps {
                script {
                    sh 'docker compose up -d'
                }
            }
        }
        
        stage('Update database') {
            steps {
                script {
                    // Change directory to src/Persistence and update the database
                    sh '''
                        cd src/Persistence
                        dotnet ef database update --startup-project ../WebApi
                    '''
                }
            }
        }
    }
}
