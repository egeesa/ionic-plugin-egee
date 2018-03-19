declare module server {
	interface setDeviceConfigurationContractRequest {
		portComEntrant: string;
		pathScript: string;
		configParams: server.ConfigParam[];
	}
}
