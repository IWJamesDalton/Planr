<template>
  <v-layout row>

    <v-flex 
      xs12 
      sm12>
      <v-date-picker
        v-model="dateString"
        full-width
        landscape
        color="cyan lighten-1"
      />
      <v-progress-circular
        v-if="!days"
        :size="70"
        :width="7"
        color="cyan"
        indeterminate
      />
      <div v-if="days" >
        <v-card 
          v-for="(day, index) in days" 
          :key="index">
          <v-list two-line>
            <template v-for="(task, index) in day.tasks" >
              <div :key="index">
                <v-subheader 
                  v-if="index == 0">
                  Most Important Task Of The Day
                </v-subheader>
                <v-subheader 
                  v-else-if="index == 1">
                  Secondary Tasks of Importance
                </v-subheader>
                <v-subheader 
                  v-else-if="index == 3">
                  Additional Tasks
                </v-subheader>
                <v-divider 
                  v-if="index > 0" 
                  :inset="true"/>
                <v-list-tile>
                  <v-layout>
                    <v-flex 
                      xs12 
                      sm10>
                      <v-text-field 
                        v-model="task.name" 
                        label="Task"/>
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
              </div>
            </template>
          </v-list>
          <v-flex 
            xs12 
            px-3>
            <v-textarea
              v-model="day.notes"
              label="Notes"
            />
            <v-subheader px-0>Productivity Score</v-subheader>
            <v-slider
              v-model="day.productivityScore"
              :max="10"
              :min="0"
              color="cyan" 
              thumb-label="always"
              step="1"
              ticks
            />
          </v-flex>
          <v-btn 
            :disabled="saving"
            color="success"
            px-3
            @click="update">Update</v-btn>
        </v-card>
      </div>
      <v-snackbar
        v-model="snackbar"
        :right="true"
        :timeout="5000"
        :top="true"
      >
        {{ snackText }}
        <v-btn
          color="pink"
          flat
          @click="snackbar = false"
        >
          Close
        </v-btn>
      </v-snackbar>
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
      saving: false,
      snackbar: false,
      date: new Date(),
      dateString: '',
      snackText: '',
      auth: {
        enabled: false
      },
      priorities: [0, 1, 2, 3, 4],
      apiBaseUrl: '',
      functionKey: '',
      api: {}
    }
    if (process.browser) {
      data.auth = {
        enabled: window.authEnabled,
        token: window.auth.token
      }
      data.apiBaseUrl = window.apiBaseUrl
      data.functionKey = window.functionKey
    }
    return data
  },
  computed: {
    loading() {
      return !this.initialized
    },
    loggedIn() {
      if (this.auth) {
        return this.auth.token
      } else {
        console.log('Auth undefined in index')
        return false
      }
    }
  },
  watch: {
    dateString(dateString) {
      const date = new Date(dateString)
      this.days = null
      this.initialized = false
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
        .catch(err => {
          if (err.request && err.request.status === 401) {
            this.$store.commit('auth/setLoggedIn', false)
            this.$router.push('/')
          }
          console.log(err)
          this.days = staticDay(date)
          this.initialized = true
        })
    },
    date(date) {
      this.dateString = date.toISOString().substr(0, 10)
      this.initialized = false
      this.days = null
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
        .catch(err => {
          if (err.request && err.request.status === 401) {
            this.$store.commit('auth/setLoggedIn', false)
            this.$router.push('/')
          }
          console.log(err)
          this.days = staticDay(this.date)
          this.initialized = true
        })
    }
  },
  mounted() {
    if (!this.auth.enabled || this.loggedIn) {
      this.api = new Api(this.apiBaseUrl, this.functionKey, this.auth.token)
      this.initialized = false
      this.days = null
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
        .catch(err => {
          if (err.request && err.request.status === 401) {
            this.$store.commit('auth/setLoggedIn', false)
            this.$router.push('/')
          }
          console.log(err)
          this.days = staticDay(this.date)
          this.initialized = true
        })
    } else {
      this.days = staticDay(this.date)
      this.initialized = true
    }
  },
  methods: {
    update() {
      this.saving = true
      return this.api
        .addDay(this.days[0])
        .then(data => {
          this.snackText = 'Day saved'
          this.snackbar = true
          console.log('200')
          this.saving = false
        })
        .catch(err => {
          if (err.request && err.request.status === 401) {
            this.$store.commit('auth/setLoggedIn', false)
            this.$router.push('/')
          }
          this.snackText = 'Error saving day'
          this.snackbar = true
          this.saving = false
          console.log('Error')
        })
    }
  }
}
</script>
