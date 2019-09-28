<template>
  <div class="container">
    <div>
      <h1 class="title">FunctionsTest.Nuxt</h1>
      <h2
        class="subtitle"
      >UI to demonstrate the efficiency of the Azure Functions I&#39;ve developed and demonstrate various concepts in a clear and easy to use manner</h2>
    </div>

    <div v-for="person of people" :key="person.id">{{ person.key }} - {{ person.name }}</div>
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, namespace } from 'nuxt-property-decorator'

import { PersonListStoreKeys } from '@/models/PersonListStoreKeys'
import { IPerson } from '@/models/IPerson'

const personListStore = namespace(PersonListStoreKeys.namespace)

@Component
export default class PersonList extends Vue {
  @personListStore.Getter(PersonListStoreKeys.getters.isLoading)
  public isLoading: boolean

  @personListStore.State(PersonListStoreKeys.state.people)
  public people: IPerson
}
</script>

<style>
.container {
  margin: 0 auto;
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  text-align: center;
}

.title {
  font-family: 'Quicksand', 'Source Sans Pro', -apple-system, BlinkMacSystemFont,
    'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  display: block;
  font-weight: 300;
  font-size: 100px;
  color: #35495e;
  letter-spacing: 1px;
}

.subtitle {
  font-weight: 300;
  font-size: 42px;
  color: #526488;
  word-spacing: 5px;
  padding-bottom: 15px;
}

.links {
  padding-top: 15px;
}
</style>
