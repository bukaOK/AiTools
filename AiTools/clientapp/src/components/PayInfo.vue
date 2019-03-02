<template>
    <div v-if="authenticated" style="margin-right: 10px">
        <v-menu>
            <v-toolbar-title slot="activator">
                {{userCash}} <v-icon>fas fa-caret-down</v-icon>
            </v-toolbar-title>
            <v-list>
                <v-list-tile @click="payInVisible = true">
                    <v-list-tile-title>
                        Пополнить счет
                    </v-list-tile-title>
                </v-list-tile>
                <v-list-tile>
                    <v-list-tile-title @click="payOutVisible = true">
                        Снять средства
                    </v-list-tile-title>
                </v-list-tile>
            </v-list>
        </v-menu>
        <pay-in-dialog :dialog-visible="payInVisible" @hide-dialog="hidePayIn" />
    </div>
</template>
<script>
import {mapState} from 'vuex';
import PayInDialog from './PayInDialog';

export default {
    name: 'pay-info',
    data(){
        return {
            payInVisible: false,
            payOutVisible: false
        }
    },
    components: {
        PayInDialog
    },
    computed: {
        ...mapState([
            'userCash',
            'authenticated'
        ])
    },
    methods: {
        hidePayIn(){
            this.payInVisible = false;
        }
    }
}
</script>

