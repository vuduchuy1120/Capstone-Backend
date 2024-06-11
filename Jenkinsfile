pipeline {
    agent any

    tools {
        dotnetsdk "DotNet8"
    }
    
    stages {

        // stage('Run unit test') {
        //     steps {
        //         script {
        //             sh 'dotnet test'
        //         }
        //     }
        // }

        stage('Remove old capstone project') {
            steps {
                script {
                    sh '''
                        docker rm -f capstone || true
                        docker image rm dihson103/capstone || true
                    '''
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
                        /var/jenkins_home/.dotnet/tools/dotnet-ef database update --startup-project ../WebApi
                    '''
                }
            }
        }
    }
}
