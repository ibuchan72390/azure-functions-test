<template>
  <div>
    <h2>Person List</h2>
    <button @click="init">Refresh</button>
    <div v-if="isLoading">Loading...</div>
    <div v-if="!isLoading">
      <div
        v-for="person of people"
        :key="person.id"
        style="border: 1px solid gray; margin: 12px; text-align: left; width: 300px;"
      >
        <div>Name: {{person.name}}</div>
        <div>Id: {{person.id}}</div>
      </div>
    </div>
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
  public async serverPrefetch() {
    await this.init()
  }

  public async mounted() {
    if (this.isLoading) {
      await this.init()
    }
  }

  @personListStore.Getter(PersonListStoreKeys.getters.isLoading)
  public isLoading: boolean

  @personListStore.State(PersonListStoreKeys.state.people)
  public people: IPerson[]

  @personListStore.Action(PersonListStoreKeys.actions.initialize)
  public init: () => Promise<void>
}
</script>
