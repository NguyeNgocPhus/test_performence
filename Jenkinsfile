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
          	branch 'master'
          }
            steps {
                echo 'Setting up permission ...'
               
            }
        }
     
    }
}
