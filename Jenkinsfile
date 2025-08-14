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
            when {
                branch 'dev'
            }
            steps {
                echo 'Setting up permission ...'
            }
        }
        
        stage('Build') {
            when {
                branch 'dev'
            }
            steps {
                echo 'Building the application...'
                script {
                   
                    sh 'docker build -t nguyenphu/testperformance:v10 .'
                    sh 'docker push nguyenphu/testperformance:v10'
                }
            }
        }
        
        stage('Test') {
            when {
                branch 'dev'
            }
            steps {
                echo 'Running tests...'
                // Add your test commands here
            }
        }
        
        stage('Deploy') {
            when {
                branch 'dev'
            }
            steps {
                echo 'Deploying the application...'
                // Add your deployment commands here
            }
        }
    }
    
}