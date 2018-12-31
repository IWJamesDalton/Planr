<template>
  <v-layout row>
    <v-flex 
      v-if="days" 
      xs12 
      sm10 
      offset-sm3>
      <v-date-picker
        v-model="dateString"
        full-width
        landscape
        color="cyan lighten-1"
      />
      <v-card 
        v-for="(day, index) in days" 
        :key="index">
        <v-toolbar 
          color="cyan" 
          dark>
          <v-toolbar-side-icon />

          <v-toolbar-title>Day</v-toolbar-title>

          <v-spacer />

          <v-btn icon> <v-icon>search</v-icon> </v-btn>
        </v-toolbar>

        <v-list two-line>
          <template v-for="(task, index) in day.tasks">
            <v-subheader 
              v-if="index == 0" 
              :key="index">
              Most Important Task Of The Day
            </v-subheader>

            <v-subheader 
              v-else-if="index == 1" 
              :key="index">
              Secondary Tasks of Importance
            </v-subheader>

            <v-subheader 
              v-else-if="index == 3" 
              :key="index">
              Additional Tasks
            </v-subheader>

            <v-divider 
              v-if="index > 0" 
              :inset="true" 
              :key="index" />

            <v-list-tile :key="task.name">
              <v-layout>
                <v-flex 
                  xs12 
                  sm10>
                  <v-text-field 
                    v-model="task.name" 
                    label="Task"
                  />
                </v-flex>
                <v-flex 
                  xs12 
                  sm2 
                  d-flex>
                  <v-select
                    v-model="task.pomodoro"
                    :items="priorities"
                    label="Pomodoro"
                  />
                </v-flex>
              </v-layout>
            </v-list-tile>
          </template>
        </v-list>
        <v-btn 
          color="success" 
          @click="update">Update</v-btn>
      </v-card>
    </v-flex>
  </v-layout>
</template>
<script>
import staticDay from '~/app/staticDay'
import Api from '~/app/Api'

export default {
  data() {
    const data = {
      days: null,
      initialized: false,
      date: new Date(),
      dateString: '',
      auth: {
        enabled: false
      },
      priorities: [0, 1, 2, 3, 4],
      apiBaseUrl: '',
      blobBaseUrl: '',
      functionKey: '',
      api: {},
      uploadEnabled: false,
      fileUploading: false,
      loaderText: 'Loading...',
      userData: 'Test'
    }
    if (process.browser) {
      data.auth = {
        enabled: window.authEnabled,
        loginUrl: window.auth.loginUrl,
        token: window.auth.token,
        logout: window.auth.logout,
        username: null
      }
      data.apiBaseUrl = window.apiBaseUrl
      data.blobBaseUrl = window.blobBaseUrl
      data.functionKey = window.functionKey
    }
    return data
  },
  computed: {
    pickerDate() {
      get: return this.date.toISOString().substr(0, 10)
      // setter
      set: newValue => {
        console.log(newValue)
      }
    },
    backendEnabled() {
      return !!this.apiBaseUrl
    },
    loading() {
      return !this.initialized || this.fileUploading
    },
    loggedIn() {
      if (this.auth) {
        return this.auth.enabled && this.auth.token
      } else {
        console.log('Auth undefined in index')
        return false
      }
    }
  },
  watch: {
    dateString(dateString) {
      const date = new Date(dateString)
      this.api
        .getDay(date)
        .then(day => {
          if (day.length === 0) {
            this.days = staticDay(date)
          } else {
            this.days = day
          }
          this.initialized = true
        })
        .catch(() => {
          this.days = staticDay(date)
          this.initialized = true
        })
    },
    date(date) {
      this.dateString = date.toISOString().substr(0, 10)
      this.api
        .getDay(date)
        .then(day => {
          if (day.length === 0) {
            this.days = staticDay(this.date)
          } else {
            this.days = day
          }
          this.initialized = true
        })
        .catch(() => {
          this.days = staticDay(this.date)
          this.initialized = true
        })
    }
  },
  mounted() {
    if ((this.backendEnabled && !this.auth.enabled) || this.loggedIn) {
      this.api = new Api(
        this.apiBaseUrl,
        this.blobBaseUrl,
        this.functionKey,
        this.auth.token
      )
      this.api
        .getDay(this.date)
        .then(day => {
          if (day.length === 0) {
            this.days = staticDay(this.date)
          } else {
            this.days = day
          }
          this.initialized = true
        })
        .catch(() => {
          this.days = staticDay(this.date)
          this.initialized = true
        })
    } else {
      this.days = staticDay(this.date)
      this.initialized = true
    }

    if (this.loggedIn) {
      this.api.getUsername().then(username => {
        this.auth.username = username
      })
    }
  },
  methods: {
    onFileUploading() {
      this.loaderText = ''
      this.uploadEnabled = false
      this.fileUploading = true
    },
    onFileUploadCompleted() {
      return this.api
        .getImages()
        .then(images => {
          this.fileUploading = false
        })
        .catch(() => {
          this.fileUploading = false
        })
    },
    onFileUploadProgress(progressText) {
      this.loaderText = progressText
    },
    update() {
      return this.api
        .addDay(this.days[0])
        .then(data => {
          console.log('200')
        })
        .catch(() => {
          console.log('Error')
        })
    }
  }
}
</script>
