pipeline {
    agent any
    environment {
         SERVICE_NAME='test'
         EVN_NAME='production'
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
                 steps{
                    echo 'Building the application...'
                   sh 'docker build -t nguyenphu/testperformance:v10 .'
                   sh 'docker push nguyenphu/testperformance:v10'
                 }
               
                // Add build commands here
            }
            stage('Test') {
                 when {
                                          	branch 'dev'
                                 }
                echo 'Running tests...'
                // Add test commands here
            }
            stage('Deploy') {
                echo 'Deploying the application...'
                // Add deployment commands here
            }
    }
}
