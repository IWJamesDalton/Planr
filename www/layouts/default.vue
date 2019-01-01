<template>
  <v-app dark>
    <v-navigation-drawer
      :mini-variant="miniVariant"
      :clipped="clipped"
      v-model="drawer"
      fixed
      app
    >
      <v-list>
        <v-list-tile
          v-for="(item, i) in menuItems"
          :to="item.to"
          :key="i"
          router
          exact
        >
          <v-list-tile-action>
            <v-icon v-html="item.icon" />
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title v-text="item.title" />
          </v-list-tile-content>
        </v-list-tile>
      </v-list>
    </v-navigation-drawer>
    <v-toolbar
      :clipped-left="clipped"
      fixed
      app
      color="cyan lighten-1"
    >
      <v-toolbar-side-icon @click="drawer = !drawer" />
      <v-btn
        icon
        @click.stop="miniVariant = !miniVariant"
      >
        <v-icon v-html="miniVariant ? 'chevron_right' : 'chevron_left'" />
      </v-btn>
      <v-btn
        icon
        @click.stop="fixed = !fixed"
      />
      <v-toolbar-title v-text="title"/>
      <v-spacer/>
      <button 
        v-if="!loggedIn" 
        class="login text-xs-right" 
        @click="login">
        Log In
      </button>
      <button 
        v-if="loggedIn" 
        class="login text-xs-right" 
        @click="logout">
        Log Out ({{ userName }})
      </button>
    </v-toolbar>
    <v-content>
      <v-container>
        <nuxt />
      </v-container>
    </v-content>
    <v-footer
      :fixed="fixed"
      app
    >
      <span>&copy; 2019</span>
    </v-footer>
  </v-app>
</template>

<script>
import { mapMutations } from 'vuex'

export default {
  data() {
    return {
      clipped: false,
      drawer: true,
      fixed: false,
      items: [
        { icon: 'apps', title: 'Home', to: '/', requireAuth: false },
        {
          icon: 'bubble_chart',
          title: 'Daily Tasks',
          to: '/daily',
          requireAuth: true
        }
      ],
      miniVariant: false,
      right: true,
      rightDrawer: false,
      title: 'Planr'
    }
  },
  computed: {
    loggedIn() {
      return this.$store.state.auth.loggedIn
    },
    userName() {
      return this.$store.state.auth.username
    },
    menuItems() {
      if (this.loggedIn) {
        return this.items
      } else {
        return this.items.filter(c => c.requireAuth === false)
      }
    }
  },
  beforeMount: function() {
    console.log(`mounting layout ${window.auth.loginUrl}`)
    this.$store.commit('auth/setLoginUrl', window.auth.loginUrl)
    if (sessionStorage.getItem('authToken')) {
      this.$store.commit('auth/setToken', sessionStorage.getItem('authToken'))
      this.$store.commit('auth/setLoggedIn', true)
      this.$store.commit('auth/loadUsername')
    }
  },
  methods: {
    login() {
      if (process.browser) {
        this.$store.commit('auth/login')
      }
    },
    logout() {
      this.$store.commit('auth/setLoggedIn', false)
      this.$router.push('/')
    }
  }
}
</script>
