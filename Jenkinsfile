pipeline {
    agent any
    environment {
        SERVICE_NAME = 'test'
        EVN_NAME = 'production'
        // DOCKER_CREDENTIALS = credentials('docker-hub-credentials') // Add this credential in Jenkins
    }
    options {
        timeout(time: 1, unit: 'HOURS')
    }
    stages {
        stage('SETTING UP PERMISSIONS PHASE') {
            steps {
                echo 'Branch is...'
                script {
                    sh 'git branch'
                }`
            
                echo 'Setting up permission ...'
            }
        }
        
        stage('Build') {
            steps {
                echo 'Building the application...'
                script {
                   
                    sh 'docker build -t nguyenphu/testperformance:v10 .'
                    sh 'docker push nguyenphu/testperformance:v10'
                }
            }
        }
        
        stage('Test') {
            steps {
                echo 'Running tests...'
                // Add your test commands here
            }
        }
        
        stage('Deploy') {
            steps {
                echo 'Deploying the application...'
                // Add your deployment commands here
            }
        }
    }
    
}