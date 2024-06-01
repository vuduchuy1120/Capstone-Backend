pipeline {
    agent any
    
    stages {
        stage('Clone project') {
            steps {
                // Clone the repository with branch specified
                git branch: 'main', url: 'https://github.com/dihson103/dotnet-cicd.git'
            }
        }

        stage('Run unit test') {
            steps {
                script {
                    sh 'dotnet test'
                }
            }
        }

        // stage('Access Jenkins Docker Container') {
        //     steps {
        //         script {
        //             // Access the Jenkins Docker container and change directory
        //             docker.image('khaliddinh/jenkins:latest').inside {
        //                 sh 'cd /var/jenkins_home/workspace/DotnetTestCiCd'
        //             }
        //         }
        //     }
        // }

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
